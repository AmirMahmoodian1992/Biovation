IF OBJECT_ID('Rst.ServeLog', 'U') IS NOT NULL 
  DROP TABLE [Rst].[ServeLog]
GO

IF OBJECT_ID('Rst.Reservation', 'U') IS NOT NULL 
  DROP TABLE [Rst].[Reservation]
GO

IF OBJECT_ID('Rst.ServeLogStatus', 'U') IS NOT NULL 
  DROP TABLE [Rst].[ServeLogStatus]
GO

IF OBJECT_ID('Rst.MealTiming', 'U') IS NOT NULL 
  DROP TABLE [Rst].[MealTiming]
GO

IF OBJECT_ID('Rst.Meal', 'U') IS NOT NULL 
  DROP TABLE [Rst].[Meal]
GO

IF OBJECT_ID('Rst.Food', 'U') IS NOT NULL 
  DROP TABLE [Rst].[Food]
GO

IF (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
    WHERE CONSTRAINT_NAME='FK_Device_Restaurant') IS NOT NULL
BEGIN
ALTER TABLE [dbo].[Device]  
DROP CONSTRAINT [FK_Device_Restaurant]
END
GO

IF COL_LENGTH('dbo.Device', 'RestaurantId') IS NOT NULL
BEGIN
ALTER TABLE dbo.[Device]
DROP COLUMN [RestaurantId]
END
GO

IF OBJECT_ID('Rst.Restaurant', 'U') IS NOT NULL 
  DROP TABLE [Rst].[Restaurant]
GO

IF OBJECT_ID('Rst.ReservationStatus', 'U') IS NOT NULL 
  DROP TABLE [Rst].[ReservationStatus]