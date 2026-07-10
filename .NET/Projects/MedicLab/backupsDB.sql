USE MedicLabDb;

BACKUP DATABASE [MedicLabDb]
TO DISK = N'/var/opt/mssql/data/MedicLabDb-Full.bak'
WITH FORMAT,
     INIT,
     NAME = N'NombreDeTuBaseDeDatos-Respaldo Completo',
     STATS = 10;
GO


USE master;
GO

-- Cierra las conexiones activas y establece la base de datos en modo de usuario único
ALTER DATABASE [MedicLabDb]
SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO

-- Restaura la base de datos
RESTORE DATABASE [MedicLabDb]
FROM DISK = N'/var/opt/mssql/data/MedicLabDb-Full.bak'
WITH REPLACE,
     STATS = 10;
GO

-- Devuelve la base de datos al modo multiusuario para que pueda ser utilizada
ALTER DATABASE [MedicLabDb]
SET MULTI_USER;
GO



USE MedicLabDb
TRUNCATE TABLE dbo.AppointmentDetails;
TRUNCATE TABLE dbo.EventLogs;
TRUNCATE TABLE dbo.FulfillmentEvents;
TRUNCATE TABLE dbo.AppointmentOrders;
GO


DBCC CHECKIDENT ('dbo.AppointmentOrders', RESEED, 0); 