
CREATE PROCEDURE [dbo].[SelectDeviceModels]
AS
BEGIN
  SELECT [DevModel].[Id],
           [DevModel].[Name],
           [DevModel].[ManufactureCode],
           [DevModel].[GetLogMethodType],
           [DevModel].[Description],
		   [DevModel].[DefaultPortNumber],
           [DevBrand].[code] AS Brand_code,
           [DevBrand].[Name] AS Brand_Name,
           [DevBrand].[Description] AS Brand_Description
    FROM   [dbo].[DeviceModel] AS DevModel
           INNER JOIN
           [dbo].[lookup] AS DevBrand
           ON DevModel.BrandId = DevBrand.code;
END
GO
