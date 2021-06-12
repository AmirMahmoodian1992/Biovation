CREATE PROCEDURE [RLY].[SelectEntrance]
@Id INT, @Name NVARCHAR(50), @SchedulingId INT, @DeviceId INT, @PageNumber INT=0, @PageSize INT=0
AS
DECLARE @HasPaging AS BIT;
DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (5) = N'200';
BEGIN
    SET @HasPaging = CASE WHEN @PageSize = 0
                               AND @PageNumber = 0 THEN 0 ELSE 1 END;
    IF @HasPaging = 1
        BEGIN
			SELECT	[Ent].[Id],
					[Ent].[Name],
					[Ent].[Description],
					[D].[Id] AS  Devices_DeviceId,
				   [D].[Code] AS  Devices_Code,
				   [D].[Active] AS  Devices_Active,
				   [D].[DeviceModelId] AS  Devices_ModelId,
				   [D].[DeviceModelId] AS  Devices_Model_Id,
				   [D].[Name] AS  Devices_Name,
				   [D].[IpAddress] AS  Devices_IpAddress,
				   [D].[Port] AS  Devices_Port,
				   [D].[MacAddress] AS  Devices_MacAddress,
				   [D].[RegisterDate] AS  Devices_RegisterDate,
				   [D].[HardwareVersion] AS  Devices_HardwareVersion,
				   [D].[FirmwareVersion] AS  Devices_FirmwareVersion,
				   [D].[DeviceTypeId] AS  Devices_DeviceTypeId,
				   [D].[DeviceLockPassword] AS  Devices_DeviceLockPassword,
				   [D].[SSL] AS  Devices_SSL,
				   [D].[TimeSync] AS  Devices_TimeSync,
				   [D].[SerialNumber] AS Devices_SerialNumber,
				   [S].[Id] AS Schedulings_Id,
				   [S].[StartTime] AS Schedulings_StartTime,
				   [S].[EndTime] AS Schedulings_EndTime,
				   [S].[Mode] Schedulings_Mode,
				    (@PageNumber - 1) * @PageSize AS [from],
                     @PageNumber AS PageNumber,
                     @PageSize AS PageSize,
                     count(*) OVER () AS [Count],
                     @Message AS e_Message,
                     @Code AS e_Code,
                     @Validate AS e_Validate
			FROM [Rly].[Entrance] AS Ent
				LEFT OUTER JOIN
				[Rly].[EntranceDevice] AS ED
				ON ED.EntranceId = Ent.[Id]
				LEFT OUTER JOIN
				[dbo].Device AS D
				ON D.Id = ED.DeviceId
				LEFT OUTER JOIN
				[Rly].[EntranceScheduling] AS ES
				ON ES.EntranceId = Ent.[Id]
				LEFT OUTER JOIN
				[Rly].Scheduling AS S
				ON S.Id = ES.SchedulingId
			WHERE   ([Ent].[Id] = @Id
						  OR ISNULL(@Id,'') = '')
					 AND ([Ent].[Name] = @Name
						  OR ISNULL(@Name,0) = 0)
   				     AND ([ES].[SchedulingId] = @SchedulingId
						  OR ISNULL(@SchedulingId,0) = 0)
					 AND ([ED].[DeviceId] = @DeviceId
						  OR ISNULL(@DeviceId,0) = 0)
			ORDER BY [Ent].Id
            OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
		END
		ELSE
			BEGIN
				SELECT [Ent].[Id],
					[Ent].[Name],
					[Ent].[Description],
					[D].[Id] AS  Devices_DeviceId,
				   [D].[Code] AS  Devices_Code,
				   [D].[Active] AS  Devices_Active,
				   [D].[DeviceModelId] AS  Devices_ModelId,
				   [D].[DeviceModelId] AS  Devices_Model_Id,
				   [D].[Name] AS  Devices_Name,
				   [D].[IpAddress] AS  Devices_IpAddress,
				   [D].[Port] AS  Devices_Port,
				   [D].[MacAddress] AS  Devices_MacAddress,
				   [D].[RegisterDate] AS  Devices_RegisterDate,
				   [D].[HardwareVersion] AS  Devices_HardwareVersion,
				   [D].[FirmwareVersion] AS  Devices_FirmwareVersion,
				   [D].[DeviceTypeId] AS  Devices_DeviceTypeId,
				   [D].[DeviceLockPassword] AS  Devices_DeviceLockPassword,
				   [D].[SSL] AS  Devices_SSL,
				   [D].[TimeSync] AS  Devices_TimeSync,
				   [D].[SerialNumber] AS Devices_SerialNumber,
				   [S].[Id] AS Schedulings_Id,
				   [S].[StartTime] AS Schedulings_StartTime,
				   [S].[EndTime] AS Schedulings_EndTime,
				   [S].[Mode] Schedulings_Mode,
				   1 AS [from],
				   1 AS PageNumber,
				   count(*) OVER () AS PageSize,
				   count(*) OVER () AS [Count],
                   @Message AS e_Message,
                   @Code AS e_Code,
                   @Validate AS e_Validate
				FROM [Rly].[Entrance] AS Ent
				LEFT OUTER JOIN
				[Rly].[EntranceDevice] AS ED
				ON ED.EntranceId = Ent.[Id]
				LEFT OUTER JOIN
				[dbo].Device AS D
				ON D.Id = ED.DeviceId
				LEFT OUTER JOIN
				[Rly].[EntranceScheduling] AS ES
				ON ES.EntranceId = Ent.[Id]
				LEFT OUTER JOIN
				[Rly].Scheduling AS S
				ON S.Id = ES.SchedulingId
			WHERE   ([Ent].[Id] = @Id
						  OR ISNULL(@Id,'') = '')
					 AND ([Ent].[Name] = @Name
						  OR ISNULL(@Name,0) = 0)
   				     AND ([ES].[SchedulingId] = @SchedulingId
						  OR ISNULL(@SchedulingId,0) = 0)
					 AND ([ED].[DeviceId] = @DeviceId
						  OR ISNULL(@DeviceId,0) = 0)
			END
	 
END