using System.Collections.Concurrent;
using MedicLab.Data;
using MedicLab.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace MedicLab.Api.Fulfillment;

public interface IFulfilmentService
{
    public Task<BurstResult> FulfillmentBurstAsync(IEnumerable<int> orderIds, CancellationToken ct);
    public Task<FulfilementResult> ProcessSingleOrderAsync(int orderId, CancellationToken ct);
}

public enum FulfilementResult { Fulfilled, Backordered}

public record BurstResult (int Fulfilled, int Backordered);

public class FulfilmentService : IFulfilmentService
{
    private readonly IDbContextFactory<MedicLabDbContext> _factory;
    private readonly BurstPlanner _planner;
    private readonly ConcurrentDictionary<string, int> _loincToStudyId;

    public FulfilmentService(IDbContextFactory<MedicLabDbContext> factory, BurstPlanner planner)
    {
        _factory = factory;
        _planner = planner;
        using var db = _factory.CreateDbContext();

        _loincToStudyId = new ConcurrentDictionary<string, int>( 
            db.ClinicalStudies.ToDictionary(cs => cs.LOINC, cs => cs.ClinicalStudyId)
        );
    }

    public async Task<FulfilementResult> ProcessSingleOrderAsync(int orderId, CancellationToken ct)
    {
        await using var db = await _factory.CreateDbContextAsync(ct);
        AppointmentOrder? appointmentOrder = await db.AppointmentOrders.Include(ao => ao.Details).FirstAsync(ao => ao.AppointmentOrderId == orderId, ct);

        ct.ThrowIfCancellationRequested();

        Dictionary<int, int> deductionsByAvailabilityId = new Dictionary<int, int>();
        bool canFulfill = true;

        foreach (AppointmentDetail appointmentDetail in appointmentOrder.Details)
        {
            Availability? availability = await db.Availability
                .Where(a => a.ClinicalStudyId == appointmentDetail.ClinicalStudyId 
                        && a.Day == appointmentDetail.Date).FirstOrDefaultAsync(ct);
            
            if (availability is null || availability.Slots < appointmentDetail.Quantity)
            {
                canFulfill = false;
                break;
            }
            deductionsByAvailabilityId[availability.AvailabilityId] = appointmentDetail.Quantity;
            availability.Slots -= appointmentDetail.Quantity;
        }

        if (!canFulfill)
        {
            db.ChangeTracker.Clear();
            AppointmentOrder? badAppointmentOrder = await db.AppointmentOrders.Include(ao => ao.Details).FirstAsync(ao => ao.AppointmentOrderId == orderId, ct);
            badAppointmentOrder.Status = Status.Backordered;
            db.FulfillmentEvents.Add( new FulfillmentEvent { AppointmentOrderId = orderId, Type = FulfilementResult.Backordered.ToString()});
            await db.SaveChangesAsync(ct);
            
            Log.Warning("Backordered {AppointmentOrderId}: insufficient stock", orderId);
            return FulfilementResult.Backordered;
        }

        appointmentOrder.Status = Status.Completed;
        appointmentOrder.CompletedUtc = DateTime.UtcNow;
        db.FulfillmentEvents.Add( new FulfillmentEvent
        {
          AppointmentOrderId = orderId, Type = FulfilementResult.Fulfilled.ToString()  
        });

        if (!await SaveWithRetryAsync(orderId, db, deductionsByAvailabilityId, ct))
        {
            db.ChangeTracker.Clear();
            AppointmentOrder staleAppointmentOrder = await db.AppointmentOrders.FirstAsync(ao => ao.AppointmentOrderId == orderId, ct);
            staleAppointmentOrder.Status = Status.Backordered;
            db.FulfillmentEvents.Add( new FulfillmentEvent { AppointmentOrderId = orderId, Type = FulfilementResult.Backordered.ToString()});
            await db.SaveChangesAsync(ct);
            Log.Warning("Backordered order {AppointmentOrderId} after concurrency retry", orderId);
            return FulfilementResult.Backordered;
        }
        Log.Information("Fulfilled AppointmentOrder: {AppointmentOrderId}, {DetailCount} details", orderId, appointmentOrder.Details.Count);
        return FulfilementResult.Fulfilled;
        
    }

    private static async Task<bool> SaveWithRetryAsync(int orderId, MedicLabDbContext db, 
        IReadOnlyDictionary<int, int> deductionsByAvailabilityId, CancellationToken ct)
    {
        int currentRetries = 1;
        int maxRetries = 3;

        while (currentRetries <= maxRetries)
        {
            try
            {
                await db.SaveChangesAsync(ct);
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Log.Warning("Attempty retry #{CurrentRetries}, appointmentOrder: {AppointmentOrderId}", currentRetries, orderId);
                foreach (var entry in ex.Entries)
                {
                    var current = await entry.GetDatabaseValuesAsync();

                    if (current is null) return false;

                    entry.OriginalValues.SetValues(current);

                    if (entry.Entity is Availability availability)
                    {
                        int freshValue = current.GetValue<int>(nameof(Availability.Slots));
                        int desiredAmount = deductionsByAvailabilityId[availability.AvailabilityId];

                        if (freshValue < desiredAmount) return false;
                        availability.Slots = freshValue - desiredAmount;
                    }
                }
                currentRetries++;
            }
        }
        return false;
    }

    
    public async Task<BurstResult> FulfillmentBurstAsync(IEnumerable<int> orderIds, CancellationToken ct)
    {
        List<int> idList = orderIds.ToList();

        List<AppointmentOrder> appointmentOrders;

        await using (var db = await _factory.CreateDbContextAsync(ct))
        {
            appointmentOrders = await db.AppointmentOrders.Where(ao => idList.Contains(ao.AppointmentOrderId)).ToListAsync();
        }

        IReadOnlyList<int> planned = _planner.OrderByPriority(appointmentOrders);

        var tasks = planned.Select(id => ProcessSingleOrderAsync(id, ct));

        var results = await Task.WhenAll(tasks);

        return new BurstResult(
            Fulfilled: results.Count(r => r == FulfilementResult.Fulfilled),
            Backordered: results.Count(r => r == FulfilementResult.Backordered)
        );
    }
}