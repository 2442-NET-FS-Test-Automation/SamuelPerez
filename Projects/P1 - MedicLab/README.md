# MedicLab - Order Fulfillment Service

MedicLab is a concurrent, high-performance laboratory appointment fulfillment system built with **ASP.NET Minimal APIs**, **Entity Framework Core**, and **SQL Server**. The application simulates the workflow of scheduling clinical studies, processing a burst of incoming appointment requests concurrently, and fulfilling them while preserving strict transactional consistency.

Unlike traditional physical inventory systems, MedicLab models **clinical studies as inventory**, where each study has a limited number of available appointment slots for a specific day. The fulfillment engine guarantees that the system will **never oversell** capacity, even when multiple requests hit the same study at the exact same millisecond.

---

## 1. Domain Model

MedicLab adapts the classical **Product–Order–Inventory** model to the healthcare domain:
* **Product → Clinical Study:** A test requested by patients (e.g., Blood Glucose, SARS-CoV-2 PCR), uniquely identified by an indexed **LOINC** code.
* **Inventory → Availability Slots:** The remaining capacity to perform a specific test on a specific date. 
* **Order → Appointment Order (Multi-line):** A patient's booking that can contain multiple clinical studies. The order is fulfilled strictly as **all-or-nothing**; if one study lacks availability, the entire appointment is marked as `Backordered`.

---

## 2. Coverage Contract Map

This table maps the required technical constraints to the exact components proving them in the codebase:

| Technique | Where to find it | Description / Proof |
| :--- | :--- | :--- |
| **EF Core Model & Annotations** | `MedicLabDbContext.cs`, `Entities/` | Uses `[Required]`, `[MaxLength]`, and Fluent API `HasCheckConstraint` & `IsUnique` indexes. |
| **DbContext in DI** | `Program.cs` | Uses `AddDbContextFactory<MedicLabDbContext>` to resolve fresh contexts per background task. |
| **Minimal API Surface** | `Program.cs` | Endpoints like `POST /appointmentOrders/burst` and `GET /inventory` using model binding. |
| **3NF, FKs & Transactions** | `FulfillmentService.cs` | Normalized schema. A single `await db.SaveChangesAsync()` commits the inventory deductions, status change, and log atomically. |
| **Concurrent Burst** | `FulfillmentService.FulfillmentBurstAsync` | Uses `Task.WhenAll` to map over the burst, spinning up multiple tasks where each order gets its own isolated `DbContext`. |
| **No Overselling (Token/Retry)** | `FulfillmentService.SaveWithRetryAsync` | Uses `byte[] RowVersion` in `Availability`. Catches `DbUpdateConcurrencyException`, reloads fresh DB values, re-evaluates slots, and retries. |
| **Priority Queue (Expedited)** | `BurstPlanner.cs` | Maps `Priority` enum to a `PriorityQueue<int, int>` to guarantee Expedited (0) orders dequeue before Normal (1) orders. |
| **Hash-based Lookups** | `FulfillmentService` constructor | Pre-loads studies into a `ConcurrentDictionary<string, int>` for fast LOINC-to-Id mapping. |
| **Sorted Report** | `Program.cs` | `GET /reports/top-study` sorts results natively via EF/SQL `OrderByDescending`. |
| **Structured Logging** | `Program.cs`, `FulfillmentService.cs` | Serilog configured to write to SQL Server `EventLogs` table using structured templates (e.g., `Log.Warning("Backordered {AppointmentOrderId}...")`). |

---

## 3. Data Structures & Big-O Complexity

* **Priority Queue (`PriorityQueue<int, int>`):** Used to sort incoming orders by priority. **Big-O: O(log N)** for `Enqueue` and `Dequeue`. A min-heap perfectly fits this requirement as we only need to extract the highest priority (lowest integer) efficiently without fully sorting the entire array up front.
* **Hash-based Lookups (`ConcurrentDictionary` / `Dictionary`):** Used to map LOINC codes and track internal quantities during fulfillment. **Big-O: O(1)** average case lookup. Fits perfectly to avoid linear O(N) database scans or in-memory array scans inside the tight concurrent loop.
* **Sorted Reports (`OrderByDescending`):** Used in the analytical endpoints. **Big-O: O(N log N)**. Since the sorting is translated to SQL Server via EF Core, the database engine executes an optimized sort algorithm, or directly reads an O(log N) B-Tree index if available.

---

## 4. Concurrency: Token vs Lock

To prevent the "overselling" race condition, MedicLab uses **Optimistic Concurrency (EF Core `RowVersion`)** rather than in-memory pessimistic locks (like C# `lock` or `Interlocked`).

* **When `lock` fits:** An in-memory lock is useful for single-instance, CPU-bound concurrent counters (like the `Bank` demo). However, it creates thread contention and completely fails if the application scales horizontally to multiple servers, as the lock only lives in one server's RAM.
* **Why Optimistic Concurrency fits here:** By letting the database act as the source of truth using a `RowVersion` byte array, we allow threads (and potentially multiple API servers) to run fully in parallel without artificial bottlenecks. We assume conflicts are the exception, not the rule. If two tasks attempt to deduct the exact same slot at the exact same millisecond, the DB rejects the loser. Our application simply catches the exception, fetches the latest state, and cleanly retries the math.

---

## 5. ACID and Isolation Reasoning

MedicLab fulfills orders inside a strict transaction boundary to satisfy the **all-or-nothing** requirement.
When `FulfillmentService` processes an order:
1. It deducts inventory for multiple studies in memory.
2. It changes the `AppointmentOrder` status.
3. It appends a `FulfillmentEvent` for the audit trail.
4. It calls `SaveChanges()`.

Because EF Core implicitly wraps `SaveChanges` in an ACID transaction, **Atomicity** and **Consistency** are guaranteed. If the app crashes right before the save, or if optimistic concurrency fails, the entire transaction rolls back. No order will ever be left half-applied (e.g., inventory deducted but order still marked as Pending).

---

## 6. Non-Key Indexes

Besides the automatic primary key indexes, MedicLab explicitly defines non-key indexes to protect business rules and speed up lookups:
* **`SKU / LOINC` (ClinicalStudy):** A unique non-clustered index (`IsUnique()`). Required to enforce domain integrity (no duplicate test codes) and to provide O(log N) lookups when clients query catalog studies by their natural string identifier.
* **`CURP / Email` (Patient):** Unique indexes to prevent duplicate patient registration and speed up analytical queries by customer.

---

## 7. Benchmark Results (Sequential vs. Concurrent)

By hitting the `POST /benchmark` endpoint with a mixed burst of 100 orders, we can observe the performance difference. The endpoint resets the inventory to the exact same baseline before each run to ensure fairness.

* **Sequential time:** 3380 ms
* **Concurrent time:** 857 ms
* **Speedup Factor:** 3.94x

**Parallelism vs. Concurrency note:** Our speedup comes from the fact that `Task.WhenAll` allows overlapping network I/O to the database (Concurrency) while the .NET Thread Pool utilizes multiple CPU cores to process the incoming payload and responses simultaneously (Parallelism).

---

## 8. API Surface & Status Codes

The API operates via Minimal API endpoints, returning the following semantic HTTP status codes:

* **202 Accepted:** Used strictly for `POST /appointmentOrders/burst`. The server accepts the payload and immediately returns 202 to keep the operator's UI responsive, acknowledging that the heavy fulfillment work is running asynchronously in a background Task.
* **200 OK:** Used for all read operations (`GET /inventory`, `GET /reports/*`) and synchronous success operations like the `POST /seed` catalog reset.
* **500 Internal Server Error:** Reserved for catastrophic, unhandled exceptions. Expected concurrency issues do not throw 500s; they are caught, logged as warnings by Serilog, and cleanly handled as `Backordered` states to preserve system stability.