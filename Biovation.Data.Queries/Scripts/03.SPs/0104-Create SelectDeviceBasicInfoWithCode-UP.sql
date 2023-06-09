
CREATE PROCEDURE [dbo].[SelectDeviceBasicInfoWithCode]
@Code INT, @DeviceBrandId INT,@AdminUserId INT = 0
AS
BEGIN
    SELECT [Dev].[Id] AS DeviceId,
		   [Dev].[Code],
           [DeviceModelId],
           [DevModel].[Id] AS Model_Id,
           [DevModel].[Name] AS Model_Name,
           [DevModel].[BrandId] AS Model_Brand_Code,
           [DevModel].[GetLogMethodType] AS Model_GetLogMethodType,
           [DevModel].[Description] AS Model_Description,
		   [DevModel].[DefaultPortNumber] AS Model_DefaultPortNumber,
           [DevBrand].[Code] AS Brand_Code,
           [DevBrand].[Name] AS Brand_Name,
           [DevBrand].[Description] AS Brand_Description,

           [Dev].[Name],
           [Active],
           [IPAddress],
           [Port],
           [MacAddress],
           [RegisterDate],
           [HardwareVersion],
           [FirmwareVersion],
           [DeviceLockPassword],
           [ManufactureCode],
           [SSL],
           [TimeSync],
           [SerialNumber],
           [DeviceTypeId]
    FROM   [dbo].[Device] AS Dev
           INNER JOIN
           [dbo].[DeviceModel] AS DevModel
           ON Dev.[DeviceModelId] = DevModel.[Id]
           INNER JOIN
           [dbo].[lookup] AS DevBrand
           ON DevModel.[BrandId] = DevBrand.[code]
    WHERE  Dev.Code = @Code
           AND DevBrand.[code] = @DeviceBrandId;
END
GO