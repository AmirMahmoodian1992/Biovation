IF COL_LENGTH('Rly.Entrance', 'DirectionTypeId') IS NULL
BEGIN
ALTER TABLE [Rly].[Entrance] ADD [DirectionTypeId] INT NULL
END
GO

IF COL_LENGTH('Rly.Entrance', 'ModeId') IS NULL
BEGIN
ALTER TABLE [Rly].[Entrance] ADD [ModeId] INT NULL
END
GO

IF COL_LENGTH('Rly.Entrance', 'DirectionTypeId') IS NULL
BEGIN
ALTER TABLE [Rly].[Entrance] ADD [DirectionTypeId] INT NULL
END
GO