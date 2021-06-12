CREATE PROCEDURE [dbo].[SelectEntrance]
@Id INT, @Name NVARCHAR(50), @SchedulingId INT, @DeviceId INT, @PageNumber INT=0, @PageSize INT=0
AS
DECLARE @HasPaging AS BIT;
DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (5) = N'200';
BEGIN
    SET @HasPaging = CASE WHEN @PageSize = 0
                               AND @PageNumber = 0 THEN 0 ELSE 1 END;
    IF @HasPaging = 1
        BEGIN
			SELECT	[Ent].[Id] AS Data_Id,
					[Ent].[Name] AS Data_Name,
					[Ent].[Description] AS Data_Description,
					[D].[Id] AS  Data_Devices_DeviceId,
				   [D].[Code] AS  Data_Devices_Code,
				   [D].[Active] AS  Data_Devices_Active,
				   [D].[DeviceModelId] AS  Data_Devices_ModelId,
				   [D].[DeviceModelId] AS  Data_Devices_Model_Id,
				   [D].[Name] AS  Data_Devices_Name,
				   [D].[IpAddress] AS  Data_Devices_IpAddress,
				   [D].[Port] AS  Data_Devices_Port,
				   [D].[MacAddress] AS  Data_Devices_MacAddress,
				   [D].[RegisterDate] AS  Data_Devices_RegisterDate,
				   [D].[HardwareVersion] AS  Data_Devices_HardwareVersion,
				   [D].[FirmwareVersion] AS  Data_Devices_FirmwareVersion,
				   [D].[DeviceTypeId] AS  Data_Devices_DeviceTypeId,
				   [D].[DeviceLockPassword] AS  Data_Devices_DeviceLockPassword,
				   [D].[SSL] AS  Data_Devices_SSL,
				   [D].[TimeSync] AS  Data_Devices_TimeSync,
				   [D].[SerialNumber] AS Data_Devices_SerialNumber,
				   [S].[Id] AS Data_Schedulings_Id,
				   [S].[StartTime] AS Data_Schedulings_StartTime,
				   [S].[EndTime] AS Data_Schedulings_EndTime,
				   [S].[Mode] AS Data_Schedulings_Mode,
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
				SELECT [Ent].[Id] AS Data_Id,
					[Ent].[Name] AS Data_Name,
					[Ent].[Description] AS Data_Description,
					[D].[Id] AS  Data_Devices_DeviceId,
				   [D].[Code] AS  Data_Devices_Code,
				   [D].[Active] AS  Data_Devices_Active,
				   [D].[DeviceModelId] AS  Data_Devices_ModelId,
				   [D].[DeviceModelId] AS  Data_Devices_Model_Id,
				   [D].[Name] AS  Data_Devices_Name,
				   [D].[IpAddress] AS  Data_Devices_IpAddress,
				   [D].[Port] AS  Data_Devices_Port,
				   [D].[MacAddress] AS  Data_Devices_MacAddress,
				   [D].[RegisterDate] AS  Data_Devices_RegisterDate,
				   [D].[HardwareVersion] AS  Data_Devices_HardwareVersion,
				   [D].[FirmwareVersion] AS  Data_Devices_FirmwareVersion,
				   [D].[DeviceTypeId] AS  Data_Devices_DeviceTypeId,
				   [D].[DeviceLockPassword] AS  Data_Devices_DeviceLockPassword,
				   [D].[SSL] AS  Data_Devices_SSL,
				   [D].[TimeSync] AS  Data_Devices_TimeSync,
				   [D].[SerialNumber] AS Data_Devices_SerialNumber,
				   [S].[Id] AS Data_Schedulings_Id,
				   [S].[StartTime] AS Data_Schedulings_StartTime,
				   [S].[EndTime] AS Data_Schedulings_EndTime,
				   [S].[Mode] AS Data_Schedulings_Mode,
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