IF OBJECT_ID('SelectDeviceModelsByBrandCode', 'P') IS NOT NULL
	DROP PROCEDURE [SelectDeviceModelsByBrandCode]
GO

CREATE PROCEDURE [dbo].[SelectDeviceModelsByBrandId]
@Id INT = 0
AS
BEGIN
        SELECT [DevModel].[Id],
                [DevModel].[Name],
                [DevModel].[ManufactureCode],
                [DevModel].[GetLogMethodType],
                [DevModel].[Description],
				[DevModel].[DefaultPortNumber],
                [DevBrand].[Code] AS Brand_Code,
                [DevBrand].[Name] AS Brand_Name,
                [DevBrand].[Description] AS Brand_Description
        FROM   [dbo].[DeviceModel] AS DevModel
                INNER JOIN
                [dbo].[lookUp] AS DevBrand
                ON DevModel.[BrandId] = DevBrand.[Code]
        WHERE  ISNULL(@Id, 0) = 0  OR
				DevBrand.[Code] = @Id
END
GO

