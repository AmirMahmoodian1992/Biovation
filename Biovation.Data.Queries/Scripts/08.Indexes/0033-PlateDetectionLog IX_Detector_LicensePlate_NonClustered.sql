
IF NOT EXISTS (
			SELECT object_id 
			FROM sys.indexes 
			WHERE NAME='IX_Detector_LicensePlate_NonClustered' AND object_id = OBJECT_ID('[dbo].[PlateDetectionLog]')
		)
BEGIN
	CREATE NONCLUSTERED INDEX [IX_Detector_LicensePlate_NonClustered] ON [dbo].[PlateDetectionLog]
	(
		[DetectorId] ASC,
		[LicensePlateId] DESC
	)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
END