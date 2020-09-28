CREATE PROCEDURE [dbo].[selectSearchedDevice]
	-- Add the parameters for the stored procedure here
	
	@Name int = null,
	@deviceModelId int = null,
	@deviceTypeId int = null,
	@AdminUserId int = 111
AS
BEGIN
	
    	DECLARE @Wstr NVARCHAR(400)=''
		, @Strsql NVARCHAR(4000)=''
		

	IF(ISNULL(@Name,0) <>0)
	  SET @Wstr+=' AND [Dev].[Id]= ' +  CAST(@Name AS NVARCHAR(100)) +  ''
    IF(ISNULL(@deviceModelId,0) <>0)
	  SET @Wstr+='  AND [Dev].[DeviceModelId] = '+CAST(@DeviceModelId AS NVARCHAR(10))+''
	  IF(ISNULL(@deviceTypeId,0) <>0)
	  SET @Wstr+=' AND [DevBrand].[Id] ='+CAST(@deviceTypeId AS NVARCHAR(10))+''
	  print @Wstr



	SET @Strsql='SELECT [Dev].[Id] AS DeviceId,
           [Dev].[Code],
           [Dev].[DeviceModelId],
           [DevModel].[Id] AS Model_Id,
           [DevModel].[Name] AS Model_Name,
           [DevModel].[BrandId] AS Model_BrandId,
           [DevModel].[GetLogMethodType] AS Model_GetLogMethodType,
           [DevModel].[Description] AS Model_Description,
           [DevBrand].[Id] AS Brand_Code,
           [DevBrand].[Name] AS Brand_Name,
           [DevBrand].[Description] AS Brand_Description,
           [Dev].[Name],
           [Dev].[Active],
           [Dev].[IPAddress],
           [Dev].[Port],
           [Dev].[MacAddress],
           [Dev].[RegisterDate],
           [Dev].[HardwareVersion],
           [Dev].[FirmwareVersion],
           [Dev].[DeviceLockPassword],
           [DevModel].[ManufactureCode],
           [Dev].[SSL],
           [Dev].[TimeSync],
           [Dev].[SerialNumber],
           [Dev].[DeviceTypeId]
    FROM   [dbo].[Device] AS [Dev] 
		   INNER JOIN
           [dbo].[DeviceModel] AS DevModel
           ON Dev.[DeviceModelId] = DevModel.[Id]
           INNER JOIN
           [dbo].[DeviceBrand] AS DevBrand
           ON DevModel.[BrandId] = DevBrand.[Id]
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
		    WHERE (isnull(' + CAST(@AdminUserId AS NVARCHAR(10))+ ' , 0) = 0
            OR AAG.UserId =' + CAST(@AdminUserId AS NVARCHAR(10)) + '
            OR EXISTS (SELECT Id
                       FROM [dbo].[USER]
                       WHERE IsMasterAdmin = 1
                              AND ID = ' + CAST(@AdminUserId AS NVARCHAR(10))+ '))'
							  + @Wstr 
		PRINT @Strsql
		
	EXEC (@Strsql)
END