CREATE PROCEDURE [dbo].[SelectEntrance]
@Id INT= NULL, @Code BIGINT = NULL, @Name NVARCHAR(50)= NULL, @SchedulingId INT= NULL, @CameraId INT= NULL, @DeviceId INT= NULL, @PageNumber INT=0, @PageSize INT=0, @Where NVARCHAR (MAX)='', @Order NVARCHAR (MAX)='', @FilterText NVARCHAR (MAX) = NULL
AS
DECLARE @HasPaging AS BIT;
DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @ResultCode AS NVARCHAR (5) = N'200';
BEGIN
    SET @HasPaging = CASE WHEN @PageSize = 0
                               AND @PageNumber = 0 THEN 0 ELSE 1 END;
	DECLARE @CountSqlQuery AS NVARCHAR (MAX);
    DECLARE @DataSqlQuery AS NVARCHAR (MAX);
    DECLARE @ParmDefinition AS NVARCHAR (MAX);
	SET @where = REPLACE(@where, 'EntranceId', 'Id');
	SET @where = REPLACE(@where, 'EntranceName', 'Name');
	SET @where = REPLACE(@where, 'SchedulingId', '[S].[Id]');
	SET @where = REPLACE(@where, 'SchedulingName', '[S].[Name]');
	SET @where = REPLACE(@where, 'CameraId', '[C].[Id]');
	SET @where = REPLACE(@where, 'CameraName', '[C].[Name]');
	SET @where = REPLACE(@where, 'DeviceId', '[Dev].[Id]');
	SET @where = REPLACE(@where, 'DeviceName', '[Dev].[Name]');
    SET @where = REPLACE(@where, 'True', '1');
    SET @where = REPLACE(@where, 'False', '0');
    SET @order = REPLACE(@order, 'Id', 'Data_Id');
    SET @Order = REPLACE(@order, 'Name', 'Data_Name');
    SET @Order = REPLACE(@order, 'SchedulingId', 'Data_Schedulings_Id');
    SET @Order = REPLACE(@order, 'CameraId', 'Data_Cameras_Id');
	SET @Order = REPLACE(@order, 'DeviceId', 'Data_Devices_DeviceId');
	SET @Order = REPLACE(@order, 'Active', 'Data_Active');
	IF ISNULL(@Order,'') = ''
		BEGIN
			SET	@Order = N'ORDER BY Data_Id DESC';
		END
    IF @HasPaging = 1
        BEGIN
			
			SELECT @DataSqlQuery = N'SELECT [Ent].[Id] AS Data_Id,
				   [Ent].[Code] AS Data_Code,
				   [Ent].[Name] AS Data_Name,
				   [Ent].[ModeId] AS Data_Mode_Code,
				   [Ent].[DirectionTypeId] AS Data_DirectionType_Code,
				   [Ent].[Description] AS Data_Description,
				   [C].[Id] AS  Data_Cameras_Id,
				   [C].[Code] AS  Data_Cameras_Code,
				   [C].[Active] AS  Data_Cameras_Active,
				   [C].[ModelId] AS  Data_Cameras_Model_Id,
				   [C].[Name] AS  Data_Cameras_Name,
				   [C].[Ip] AS  Data_Cameras_Ip,
				   [C].[Port] AS  Data_Cameras_Port,
				   [C].[MacAddress] AS  Data_Cameras_MacAddress,
				   [C].[RegisterDate] AS  Data_Cameras_RegisterDate,
				   [C].[HardwareVersion] AS  Data_Cameras_HardwareVersion,
				   [C].[BrandCode] AS  Data_Cameras_Brand_Code,
				   [C].[UserName] AS  Data_Cameras_UserName,
				   [C].[Password] AS  Data_Cameras_Password,
				   [C].[ConnectionUrl] AS  Data_Cameras_ConnectionUrl,
				   [C].[LiveStreamUrl] AS  Data_Cameras_LiveStreamUrl,
				   [C].[SerialNumber] AS Data_Cameras_SerialNumber,
				   [Dev].[Id] AS Data_Devices_DeviceId,
                     [Dev].[Code] AS Data_Devices_Code,
                     [dev].[DeviceModelId] AS Data_Devices_DeviceModelId,
                     [DevModel].[Id] AS Data_Devices_Model_Id,
                     [DevModel].[Name] AS Data_Devices_Model_Name,
                     [DevModel].[GetLogMethodType] AS Data_Devices_Model_GetLogMethodType,
                     [DevModel].[Description] AS Data_Devices_Model_Description,
                     [DevModel].[DefaultPortNumber] AS Data_Devices_Model_DefaultPortNumber,
                     [DevModel].[BrandId] AS Data_Devices_Model_Brand_Code,
                     [DevBrand].[Name] AS Data_Devices_Model_Brand_Name,
                     [DevBrand].[Description] AS Data_Devices_Model_Brand_Description,
                     [DevBrand].[Code] AS Data_Devices_Brand_Code,
                     [DevBrand].[Name] AS Data_Devices_Brand_Name,
                     [DevBrand].[Description] AS Data_Devices_Brand_Description,
                     [Dev].[Name] AS Data_Devices_Name,
                     [dev].[Active] AS Data_Devices_Active,
                     [dev].[IPAddress] AS Data_Devices_IPAddress,
                     [dev].[Port] AS Data_Devices_Port,
                     [dev].[MacAddress] AS Data_Devices_MacAddress,
                     [dev].[RegisterDate] AS Data_Devices_RegisterDate,
                     [dev].[HardwareVersion] AS Data_Devices_HardwareVersion,
                     [dev].[FirmwareVersion] AS Data_Devices_FirmwareVersion,
                     [dev].[DeviceLockPassword] AS Data_Devices_DeviceLockPassword,
                     [DevModel].[ManufactureCode] AS Data_Devices_ManufactureCode,
                     [dev].[SSL] AS Data_Devices_SSL,
                     [dev].[TimeSync] AS Data_Devices_TimeSync,
                     [dev].[SerialNumber] AS Data_Devices_SerialNumber,
                     [dev].[DeviceTypeId] AS Data_Devices_DeviceTypeId,
				     [S].[Id] AS Data_Schedulings_Id,
                     [S].[StartTime] AS Data_Schedulings_StartTime,
                     [S].[EndTime] AS Data_Schedulings_EndTime,
					 [S].[Mode] AS Data_Schedulings_Mode_Code,
					 [SM].[Description] Data_Schedulings_Mode_Description,
					 [SM].[Name] Data_Schedulings_Mode_Name,
					 [SM].[OrderIndex] Data_Schedulings_Mode_OrderIndex,
					 [SM].[LookupCategoryId] Data_Schedulings_Mode_Category_Id,
				    (@PageNumber - 1) * @PageSize AS [from],
                     @PageNumber AS PageNumber,
                     @PageSize AS PageSize,
                     count(*) OVER () AS [Count],
                     @Message AS e_Message,
                     @ResultCode AS e_Code,
                     @Validate AS e_Validate
			FROM [Rly].[Entrance] AS Ent
				LEFT OUTER JOIN
				[Rly].[EntranceCamera] AS EC
				ON EC.EntranceId = Ent.[Id]
				LEFT OUTER JOIN
				[Rly].Camera AS C
				ON C.Id = EC.CameraId
				LEFT OUTER JOIN
				[Rly].[EntranceScheduling] AS ES
				ON ES.EntranceId = Ent.[Id]
				LEFT OUTER JOIN
				[Rly].Scheduling AS S
				ON S.Id = ES.SchedulingId
				LEFT JOIN
				[dbo].[Lookup] AS SM
				ON [SM].Code = [S].Mode
				LEFT JOIN
				[Rly].EntranceDevice AS ED
				ON [ENT].Id = [ED].EntranceId
				LEFT JOIN
                [dbo].[Device] AS Dev
                ON [Dev].[Id] = [ED].[DeviceId]
                LEFT JOIN
                [dbo].[DeviceModel] AS DevModel
                ON Dev.[DeviceModelId] = DevModel.[Id]
                LEFT JOIN
                [dbo].[LookUp] AS DevBrand
                ON DevModel.[BrandId] = DevBrand.[Code]
                LEFT OUTER JOIN
                [dbo].[DeviceGroupMember] AS dgm
                ON dgm.DeviceId = dev.Id
                LEFT OUTER JOIN
                [dbo].[DeviceGroup] AS dg
                ON dg.Id = dgm.GroupId
                LEFT OUTER JOIN
                [dbo].[AccessGroupDevice] AS agd
                ON agd.DeviceGroupId = dg.Id
                LEFT OUTER JOIN
                [dbo].[AccessGroup] AS ag
                ON ag.Id = agd.AccessGroupId
                LEFT OUTER JOIN
                [dbo].[AdminAccessGroup] AS AAG
                ON ag.Id = AAG.AccessGroupId
			WHERE   ([Ent].[Id] = @Id
						  OR ISNULL(@Id,0) = 0)
					 AND ([Ent].[Name] = @Name
						  OR ISNULL(@Name,'''') = '''')
   				     AND ([ES].[SchedulingId] = @SchedulingId
						  OR ISNULL(@SchedulingId,0) = 0)
					 AND ([EC].[CameraId] = @CameraId
						  OR ISNULL(@CameraId,0) = 0)
					 AND ([ED].[DeviceId] = @DeviceId
						  OR ISNULL(@DeviceId,0) = 0)
					 AND ([Ent].[Code] = @Code
						  OR ISNULL(@Code,0) = 0)
					 AND((@FilterText IS NULL)
						   OR ([Ent].[Name] LIKE ''%'' + @FilterText + ''%'')
						   OR ([Ent].[Code] LIKE ''%'' + @FilterText + ''%'')
						   OR ([C].[Name] LIKE ''%'' + @FilterText + ''%'')
						   OR ([Dev].[Name] LIKE ''%'' + @FilterText + ''%''))
						   ' + ISNULL(@Where,'') + @Order
			SET @ParmDefinition = N'@Id int, @Code BigInt, @Name nvarchar(MAX), @SchedulingId INT, @CameraId INT, @DeviceId INT
								,@FilterText NVARCHAR(MAX), @PageNumber int, @PageSize int, @Message AS NVARCHAR (200), @Validate int, @ResultCode nvarchar(5)';
                    EXECUTE sp_executesql @DataSqlQuery, @ParmDefinition, @Id = @Id, @Code = @Code, @Name = @Name, @SchedulingId = @SchedulingId, @CameraId= @CameraId, @DeviceId= @DeviceId, @PageNumber = @PageNumber, @PageSize = @PageSize, @FilterText = @FilterText, @Message = @Message, @Validate = @Validate, @ResultCode = @ResultCode;
		END
		ELSE
			BEGIN
				SET @DataSqlQuery = N'SELECT [Ent].[Id] AS Data_Id,
				   [Ent].[Code] AS Data_Code,
				   [Ent].[Name] AS Data_Name,
				   [Ent].[ModeId] AS Data_Mode_Code,
				   [Ent].[DirectionTypeId] AS Data_DirectionType_Code,
				   [Ent].[Description] AS Data_Description,
				   [C].[Id] AS  Data_Cameras_Id,
				   [C].[Code] AS  Data_Cameras_Code,
				   [C].[Active] AS  Data_Cameras_Active,
				   [C].[ModelId] AS  Data_Cameras_Model_Id,
				   [C].[Name] AS  Data_Cameras_Name,
				   [C].[Ip] AS  Data_Cameras_Ip,
				   [C].[Port] AS  Data_Cameras_Port,
				   [C].[MacAddress] AS  Data_Cameras_MacAddress,
				   [C].[RegisterDate] AS  Data_Cameras_RegisterDate,
				   [C].[HardwareVersion] AS  Data_Cameras_HardwareVersion,
				   [C].[BrandCode] AS  Data_Cameras_Brand_Code,
				   [C].[UserName] AS  Data_Cameras_UserName,
				   [C].[Password] AS  Data_Cameras_Password,
				   [C].[ConnectionUrl] AS  Data_Cameras_ConnectionUrl,
				   [C].[LiveStreamUrl] AS  Data_Cameras_LiveStreamUrl,
				   [C].[SerialNumber] AS Data_Cameras_SerialNumber,
				   [Dev].[Id] AS Data_Devices_DeviceId,
                     [Dev].[Code] AS Data_Devices_Code,
                     [dev].[DeviceModelId] AS Data_Devices_DeviceModelId,
                     [DevModel].[Id] AS Data_Devices_Model_Id,
                     [DevModel].[Name] AS Data_Devices_Model_Name,
                     [DevModel].[GetLogMethodType] AS Data_Devices_Model_GetLogMethodType,
                     [DevModel].[Description] AS Data_Devices_Model_Description,
                     [DevModel].[DefaultPortNumber] AS Data_Devices_Model_DefaultPortNumber,
                     [DevModel].[BrandId] AS Data_Devices_Model_Brand_Code,
                     [DevBrand].[Name] AS Data_Devices_Model_Brand_Name,
                     [DevBrand].[Description] AS Data_Devices_Model_Brand_Description,
                     [DevBrand].[Code] AS Data_Devices_Brand_Code,
                     [DevBrand].[Name] AS Data_Devices_Brand_Name,
                     [DevBrand].[Description] AS Data_Devices_Brand_Description,
                     [Dev].[Name] AS Data_Devices_Name,
                     [dev].[Active] AS Data_Devices_Active,
                     [dev].[IPAddress] AS Data_Devices_IPAddress,
                     [dev].[Port] AS Data_Devices_Port,
                     [dev].[MacAddress] AS Data_Devices_MacAddress,
                     [dev].[RegisterDate] AS Data_Devices_RegisterDate,
                     [dev].[HardwareVersion] AS Data_Devices_HardwareVersion,
                     [dev].[FirmwareVersion] AS Data_Devices_FirmwareVersion,
                     [dev].[DeviceLockPassword] AS Data_Devices_DeviceLockPassword,
                     [DevModel].[ManufactureCode] AS Data_Devices_ManufactureCode,
                     [dev].[SSL] AS Data_Devices_SSL,
                     [dev].[TimeSync] AS Data_Devices_TimeSync,
                     [dev].[SerialNumber] AS Data_Devices_SerialNumber,
                     [dev].[DeviceTypeId] AS Data_Devices_DeviceTypeId,
				     [S].[Id] AS Data_Schedulings_Id,
                     [S].[StartTime] AS Data_Schedulings_StartTime,
                     [S].[EndTime] AS Data_Schedulings_EndTime,
					 [S].[Mode] AS Data_Schedulings_Mode_Code,
					 [SM].[Description] Data_Schedulings_Mode_Description,
					 [SM].[Name] Data_Schedulings_Mode_Name,
					 [SM].[OrderIndex] Data_Schedulings_Mode_OrderIndex,
					 [SM].[LookupCategoryId] Data_Schedulings_Mode_Category_Id,
				   1 AS [from],
				   1 AS PageNumber,
				   count(*) OVER () AS PageSize,
				   count(*) OVER () AS [Count],
                   @Message AS e_Message,
                   @ResultCode AS e_Code,
                   @Validate AS e_Validate
				FROM [Rly].[Entrance] AS Ent
				LEFT OUTER JOIN
				[Rly].[EntranceCamera] AS EC
				ON EC.EntranceId = Ent.[Id]
				LEFT OUTER JOIN
				[Rly].Camera AS C
				ON C.Id = EC.CameraId
				LEFT OUTER JOIN
				[Rly].[EntranceScheduling] AS ES
				ON ES.EntranceId = Ent.[Id]
				LEFT OUTER JOIN
				[Rly].Scheduling AS S
				ON S.Id = ES.SchedulingId
				LEFT JOIN
				[dbo].[Lookup] AS SM
				ON [SM].Code = [S].Mode
				LEFT JOIN
				[Rly].EntranceDevice AS ED
				ON [ENT].Id = [ED].EntranceId
				LEFT JOIN
                [dbo].[Device] AS Dev
                ON [Dev].[Id] = [ED].[DeviceId]
                LEFT JOIN
                [dbo].[DeviceModel] AS DevModel
                ON Dev.[DeviceModelId] = DevModel.[Id]
                LEFT JOIN
                [dbo].[LookUp] AS DevBrand
                ON DevModel.[BrandId] = DevBrand.[Code]
                LEFT OUTER JOIN
                [dbo].[DeviceGroupMember] AS dgm
                ON dgm.DeviceId = dev.Id
                LEFT OUTER JOIN
                [dbo].[DeviceGroup] AS dg
                ON dg.Id = dgm.GroupId
                LEFT OUTER JOIN
                [dbo].[AccessGroupDevice] AS agd
                ON agd.DeviceGroupId = dg.Id
                LEFT OUTER JOIN
                [dbo].[AccessGroup] AS ag
                ON ag.Id = agd.AccessGroupId
                LEFT OUTER JOIN
                [dbo].[AdminAccessGroup] AS AAG
                ON ag.Id = AAG.AccessGroupId
			WHERE   ([Ent].[Id] = @Id
						  OR ISNULL(@Id,0) = 0)
					 AND ([Ent].[Name] = @Name
						  OR ISNULL(@Name,'''') = '''')
   				     AND ([ES].[SchedulingId] = @SchedulingId
						  OR ISNULL(@SchedulingId,0) = 0)
					 AND ([EC].[CameraId] = @CameraId
						  OR ISNULL(@CameraId,0) = 0)
					 AND ([ED].[DeviceId] = @DeviceId
						  OR ISNULL(@DeviceId,0) = 0)
					 AND ([Ent].[Code] = @Code
						  OR ISNULL(@Code,0) = 0)
					  AND((@FilterText IS NULL)
						   OR ([Ent].[Name] LIKE ''%'' + @FilterText + ''%'')
						   OR ([Ent].[Code] LIKE ''%'' + @FilterText + ''%'')
						   OR ([C].[Name] LIKE ''%'' + @FilterText + ''%'')
						   OR ([Dev].[Name] LIKE ''%'' + @FilterText + ''%''))
					' + ISNULL(@Where,'') + ISNULL(@Order,'')
						  SET @ParmDefinition = N'@Id int, @Code BigInt, @Name nvarchar(MAX), @SchedulingId INT, @CameraId INT, @DeviceId INT
								,@FilterText NVARCHAR(MAX), @Message AS NVARCHAR (200), @Validate int, @ResultCode nvarchar(5)';
				   EXECUTE sp_executesql @DataSqlQuery, @ParmDefinition, @Id = @Id, @Code = @Code, @Name = @Name, @SchedulingId = @SchedulingId, @CameraId= @CameraId, @DeviceId= @DeviceId, @FilterText = @FilterText, @Message = @Message, @Validate = @Validate, @ResultCode = @ResultCode;
			END
	 
END