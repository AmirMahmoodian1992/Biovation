CREATE PROCEDURE [dbo].[SelectDeviceOfAccessGroup]
	@Id int	
AS
BEGIN
			SELECT [Dev].[Id] AS DeviceId,
				   [Dev].[Code],
				   [dev].[DeviceModelId],
				   [DevModel].[Id] AS Model_Id,
				   [DevModel].[Name] AS Model_Name,
				   [DevModel].[BrandId] AS Model_Brand_code,
				   [DevModel].[GetLogMethodType] AS Model_GetLogMethodType,
				   [DevModel].[Description] AS Model_Description,
				   [DevBrand].[code] AS Brand_code,
				   [DevBrand].[Name] AS Brand_Name,
				   [DevBrand].[Description] AS Brand_Description,
				   [Dev].[Name],
				   [dev].[Active],
				   [dev].[IPAddress],
				   [dev].[Port],
				   [dev].[MacAddress],
				   [dev].[RegisterDate],
				   [dev].[HardwareVersion],
				   [dev].[FirmwareVersion],
				   [dev].[DeviceLockPassword],
				   [DevModel].[ManufactureCode],
				   [dev].[SSL],
				   [dev].[TimeSync],
				   [dev].[SerialNumber],
				   [dev].[DeviceTypeId]
					FROM AccessGroup AG 
		INNER JOIN AccessGroupDevice AGD ON AG.Id = AGD.AccessGroupId
		INNER JOIN DeviceGroup DG ON DG.Id = AGD.DeviceGroupId
		INNER JOIN DeviceGroupMember DGM ON DGM.GroupId = DG.Id
		INNER JOIN Device DEV ON DEV.Id = DGM.DeviceId
		INNER JOIN [dbo].[DeviceModel] AS DevModel ON Dev.[DeviceModelId] = DevModel.[Id]
		INNER JOIN [dbo].[lookup] AS DevBrand  ON DevModel.[BrandId] = DevBrand.[code]
		WHERE AG.Id = @ID

END