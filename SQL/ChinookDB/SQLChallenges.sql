-- Parking Lot*******
-- *                *
-- *                *
--- *****************



-- Comment can be done single line with --
-- Comment can be done multi line with /* */

/*
DQL - Data Query Language
Keywords:

SELECT - retrieve data, select the columns from the resulting set
FROM - the table(s) to retrieve data from
WHERE - a conditional filter of the data
GROUP BY - group the data based on one or more columns
HAVING - a conditional filter of the grouped data
ORDER BY - sort the data
*/


-- BASIC CHALLENGES
-- List all customers (full name, customer id, and country) who are not in the USA

SELECT FirstName + '' + LastName AS FullName,
CustomerId, Country
FROM Customer 
WHERE Country NOT LIKE 'USA'; 

-- List all customers from Brazil
SELECT FirstName + '' + LastName AS FullName,
CustomerId, Country
FROM Customer 
WHERE Country LIKE 'BRAZIL'; 


-- List all sales agents

SELECT FirstName + '' + LastName AS FullName,
Title
FROM Employee
WHERE Title LIKE '%Agent%';


-- Retrieve a list of all countries in billing addresses on invoices

SELECT DISTINCT(BillingCountry) AS Country
FROM Invoice;


-- Retrieve how many invoices there were in 2021, and what was the sales total for that year?
SELECT COUNT(*) AS Total_Invoices,  SUM(Total) Total_Year
FROM Invoice
WHERE YEAR(InvoiceDate) = 2021


-- (challenge: find the invoice count sales total for every year using one query)

SELECT YEAR(InvoiceDate) AS Year, SUM(Total) Total_Year
FROM Invoice
GROUP BY YEAR(InvoiceDate);


-- how many line items were there for invoice #37
SELECT InvoiceId, COUNT(*) AS Total_Lines
FROM InvoiceLine
WHERE InvoiceId = 37
GROUP BY InvoiceId;


-- how many invoices per country? BillingCountry  # of invoices 
SELECT BillingCountry, COUNT(*) AS Invoices_per_Country
FROM Invoice
GROUP BY BillingCountry;


-- Retrieve the total sales per country, ordered by the highest total sales first.
SELECT BillingCountry, COUNT(*) AS Invoices_per_Country, SUM(Total) AS Total
FROM Invoice
GROUP BY BillingCountry
ORDER BY Total DESC;




-- JOINS CHALLENGES
-- Every Album by Artist
SELECT b.Name as Artist, a.Title as Album
FROM Album a 
JOIN Artist b ON a.ArtistId = b.ArtistId


-- (inner keyword is optional for inner join)

-- All songs of the rock genre
SELECT a.Name, b.Name 
FROM Track a
JOIN Genre b ON a.GenreId = b.GenreId
WHERE b.Name LIKE 'Rock';


-- Show all invoices of customers from brazil (mailing address not billing)
SELECT b.*
FROM Customer a
JOIN Invoice b ON a.CustomerId = b.CustomerId
WHERE a.Country LIKE 'Brazil'


-- Show all invoices together with the name of the sales agent for each one
SELECT c.FirstName, a.InvoiceId
FROM Invoice a
JOIN Customer b
ON a.CustomerId = b.CustomerId
JOIN Employee c 
ON b.SupportRepId = c.EmployeeId
GROUP BY c.FirstName, a.InvoiceId
ORDER BY FirstName, InvoiceId DESC;



-- Which sales agent made the most sales in 2021?
SELECT TOP 1 c.EmployeeId, c.FirstName,  SUM(a.Total) as total, COUNT(*) AS Total_invoices
FROM Invoice a
JOIN Customer b
ON a.CustomerId = b.CustomerId
JOIN Employee c 
ON b.SupportRepId = c.EmployeeId
WHERE YEAR(a.InvoiceDate) = 2021
GROUP BY c.EmployeeId, c.FirstName
ORDER BY total DESC, FirstName;


-- How many customers are assigned to each sales agent?
SELECT b.EmployeeId, b.FirstName, COUNT(*) AS Total_Customers_Assigned
FROM Customer a 
JOIN Employee b
ON a.SupportRepId = b.EmployeeId
GROUP BY b.EmployeeId, b.FirstName;



-- Which track was purchased the most in 2022 
SELECT TOP 1 
    c.TrackId,
    c.Name AS TrackName,
    SUM(a.Quantity) AS TotalPurchased
FROM InvoiceLine a
JOIN Invoice b ON a.InvoiceId = b.InvoiceId
JOIN Track c ON a.TrackId = c.TrackId
WHERE YEAR(b.InvoiceDate) = 2023
GROUP BY c.TrackId, c.Name
ORDER BY TotalPurchased DESC;
 
-- Show the top three best selling artists.
SELECT e.Name, COUNT(*) AS Total_sales
FROM Invoice a
JOIN InvoiceLine b
ON a.InvoiceId = b.InvoiceId
JOIN Track c
ON b.TrackId = c.TrackId
JOIN Album d
ON c.AlbumId = d.AlbumId
JOIN Artist e
ON d.ArtistId = e.ArtistId 
GROUP BY e.ArtistId, e.Name
ORDER BY Total_sales DESC;


-- Which customers have the same initials as at least one other customer?
SELECT a.*
FROM Customer a
WHERE EXISTS (
    SELECT 1
    FROM Customer b
    WHERE LEFT(a.FirstName, 1) = LEFT(b.FirstName, 1)
    AND LEFT(a.LastName, 1) = LEFT(b.LastName, 1)
    AND a.CustomerId <> b.CustomerId
)
ORDER BY a.FirstName


-- Which countries have the most invoices?
SELECT TOP 3 BillingCountry, COUNT(*) Total_Invoices
FROM Invoice
GROUP BY BillingCountry
ORDER BY Total_Invoices DESC;



-- Which city has the customer with the highest sales total?
SELECT TOP 1 CustomerId, SUM(Total) AS Sales_Total, BillingCity
FROM Invoice
GROUP BY CustomerId, BillingCity
ORDER BY Sales_Total DESC


-- Who is the highest spending customer?
SELECT TOP 1 Invoice.CustomerId, FirstName, SUM(Total) AS Sales_Total
FROM Invoice
JOIN Customer
ON Invoice.CustomerId = Customer.CustomerId
GROUP BY Invoice.CustomerId, FirstName
ORDER BY Sales_Total DESC


-- Return the email and full name of of all customers who listen to Rock.

SELECT a.Email, a.FirstName + ' ' + a.LastName as Full_Name
FROM Customer a
JOIN Invoice b ON a.CustomerId = b.CustomerId
JOIN InvoiceLine c ON b.InvoiceId = c.InvoiceId
JOIN Track d ON c.TrackId = d.TrackId
JOIN Genre e ON d.GenreId = e.GenreId
WHERE e.Name LIKE 'Rock'
GROUP BY a.Email, a.FirstName, a.LastName


-- Which artist has written the most Rock songs?
SELECT TOP 1 c.Name, COUNT(*) AS Total_Written
FROM Track a
JOIN Album b ON a.AlbumId = b.AlbumId
JOIN Artist c ON b.ArtistId = c.ArtistId
JOIN Genre d ON a.GenreId = d.GenreId
WHERE d.Name LIKE 'Rock'
GROUP BY c.Name
ORDER BY Total_Written DESC


-- Which artist has generated the most revenue?

SELECT TOP 1 e.ArtistId, e.Name, SUM(b.UnitPrice * b.Quantity) AS Total_Revenue
FROM Invoice a
JOIN InvoiceLine b ON a.InvoiceId = b.InvoiceId
JOIN Track c ON b.TrackId = c.TrackId
JOIN Album d ON c.AlbumId = d.AlbumId
JOIN Artist e ON d.ArtistId = e.ArtistId
GROUP BY e.ArtistId, e.Name
ORDER BY Total_Revenue DESC




-- ADVANCED CHALLENGES
-- solve these with a mixture of joins, subqueries, CTE, and set operators.
-- solve at least one of them in two different ways, and see if the execution
-- plan for them is the same, or different.

-- 1. which artists did not make any albums at all?
WITH CTE1 AS(
    SELECT c.ArtistId as ArtistId
    FROM Track a
    JOIN Album b ON a.AlbumId = b.AlbumId
    JOIN Artist c ON b.ArtistId = c.ArtistId
    GROUP BY c.ArtistId

)
SELECT a.Name, a.ArtistId
FROM Artist a
WHERE a.ArtistId NOT IN (SELECT ArtistId FROM CTE1)




SELECT a.Name
FROM Artist a
FULL OUTER JOIN Album b ON a.ArtistId = b.ArtistId
WHERE b.ArtistId IS NULL; 




-- 2. which artists did not record any tracks of the Latin genre?


-- 3. which video track has the longest length? (use media type table)



-- 4. boss employee (the one who reports to nobody)


-- 5. how many audio tracks were bought by German customers, and what was
--    the total price paid for them?



-- 6. list the names and countries of the customers supported by an employee
--    who was hired younger than 35.




-- DML exercises

-- 1. insert two new records into the employee table.
INSERT INTO Employee VALUES ('Perez', 'Samuel', 'Sales Manager', 2, 2003-03-28, GETDATE(), 'My House', 'Zapopan', 'Jalisco', 'Mexico', '45023', '+52 (33) 2845-8834', '+52 (33) 2845-8834', 'myemail@google.com')
INSERT INTO Employee VALUES ('Gomez', 'Jorge', 'IT Staff', 2, 2001-06-17, GETDATE(), 'Your House', 'Tlaquepaque', 'Jalisco', 'Mexico', '45076', '+52 (33) 8567-2390', '+52 (33) 8567-2390', 'myemail@yahoo.com')

-- 2. insert two new records into the tracks table.
INSERT INTO Track VALUES ('Million Years Ago', 270, 3, 5, 'Samuel', 270000, 8, 0.99)
INSERT INTO Track VALUES ('In the End', 1, 4, 1, 'Luis', 310000, 16, 0.99)

-- 3. update customer Aaron Mitchell's name to Robert Walter
UPDATE Customer SET FirstName = 'Robert', LastName = 'Walter' WHERE FirstName = 'Aaron' AND LastName = 'Mitchell' 

-- 4. delete one of the employees you inserted.
DELETE FROM Employee WHERE FirstName = 'Jorge' AND LastName = 'Gomez' AND YEAR(BirthDate) = 2001

-- 5. delete customer Robert Walter.
DELETE FROM Customer WHERE FirstName = 'Robert' AND LastName = 'Walter';
