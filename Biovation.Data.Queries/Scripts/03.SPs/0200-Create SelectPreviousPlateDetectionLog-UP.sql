CREATE PROCEDURE [dbo].[SelectPreviousPlateDetectionLog]
@id INT = NULL, @logDateTime DateTime = null, @licensePlateNumber nvarchar(50) = NULL
AS
	DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (15) = N'200';
BEGIN
    IF (ISNULL(@licensePlateNumber, '') = '')
    BEGIN
		SELECT TOP 1
			PL.[Id] AS Id
			,[DetectorId] AS DetectorId
			,[C].[Name] AS DeviceName
			,[LicensePlate].[EntityId] AS LicensePlate_EntityId
			,[LicensePlate].[LicensePlate] AS LicensePlate_LicensePlateNumber
			,[EventId] AS EventLog_Code
			,[LogDateTime] AS LogDateTime
			,[DetectionPrecision]  AS DetectionPrecision
			,[SuccessTransfer] AS SuccessTransfer
			,[InOrOut] AS InOrOut
			,[LogEventLookUp].[Name] AS EventLog_Name
			,[LogEventLookUp].[OrderIndex] AS EventLog_OrderIndex
			,[LogEventLookUp].[Description] AS EventLog_Description
			FROM [dbo].[PlateDetectionLog] AS PL
			LEFT OUTER JOIN
			[dbo].[Lookup] AS [LogEventLookUp]
			ON PL.[EventId]  = [LogEventLookUp].[Code]
			LEFT OUTER JOIN
			[Rly].[Camera] AS C
			ON PL.[DetectorId]  = [C].[Id]
			LEFT OUTER JOIN
			[dbo].[LicensePlate] AS [LicensePlate]
			ON PL.LicensePlateId  = [LicensePlate].[EntityId]
                WHERE ([LicensePlateId] = (SELECT [EntityId] FROM [dbo].[LicensePlate] WHERE [EntityId] = (SELECT [LicensePlateId] FROM [dbo].[PlateDetectionLog] WHERE [Id] = @id) AND [LogDateTime] < (SELECT [LogDateTime] FROM [dbo].[PlateDetectionLog] WHERE [Id] = @id)))
                    ORDER BY [LogDateTime] DESC
    END
    ELSE
    BEGIN
        SELECT TOP 1
			PL.[Id] AS Id
			,[DetectorId] AS DetectorId
			,[C].[Name] AS DeviceName
			,[LicensePlate].[EntityId] AS LicensePlate_EntityId
			,[LicensePlate].[LicensePlate] AS LicensePlate_LicensePlateNumber
			,[EventId] AS EventLog_Code
			,[LogDateTime] AS LogDateTime
			,[DetectionPrecision]  AS DetectionPrecision
			,[SuccessTransfer] AS SuccessTransfer
			,[InOrOut] AS InOrOut
			,[LogEventLookUp].[Name] AS EventLog_Name
			,[LogEventLookUp].[OrderIndex] AS EventLog_OrderIndex
			,[LogEventLookUp].[Description] AS EventLog_Description
			,[C].[Name]
			FROM [dbo].[PlateDetectionLog] AS PL
			LEFT OUTER JOIN
			[dbo].[Lookup] AS [LogEventLookUp]
			ON PL.[EventId]  = [LogEventLookUp].[Code]
			LEFT OUTER JOIN
			[Rly].[Camera] AS C
			ON PL.[DetectorId]  = [C].[Id]
			LEFT OUTER JOIN
			[dbo].[LicensePlate] AS [LicensePlate]
			ON PL.LicensePlateId  = [LicensePlate].[EntityId]
				WHERE [LicensePlateId] = (SELECT [EntityId] FROM [dbo].[LicensePlate] WHERE [LicensePlate] = @licensePlateNumber) AND [LogDateTime] < @logDateTime 
					ORDER BY [LogDateTime] DESC
    END
END
GO
