IF COL_LENGTH('dbo.PlateDetectionLog', 'InOrOut') IS NULL
	BEGIN
		ALTER TABLE [dbo].[PlateDetectionLog]
		ADD [InOrOut] TINYINT NULL
	END
