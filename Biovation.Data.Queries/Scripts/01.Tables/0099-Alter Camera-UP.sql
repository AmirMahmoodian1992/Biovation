IF COL_LENGTH('Rly.Camera', 'ResolutionCode') IS NULL
BEGIN
ALTER TABLE [Rly].[Camera] ADD [ResolutionCode] INT NULL
END
GO