
CREATE PROCEDURE [dbo].[SelectDeviceModelByName]
@Name VARCHAR (100)
AS
BEGIN
    IF EXISTS (SELECT Code
               FROM   [lookup]
               WHERE  Name = @Name)
        BEGIN
            SELECT [DevModel].[Id],
                   [DevModel].[Name],
                   [DevModel].[ManufactureCode],
                   [DevModel].[GetLogMethodType],
                   [DevModel].[Description],
				   [DevModel].[DefaultPortNumber],
                   [DevBrand].[code] AS Brand_Code,
                   [DevBrand].[Name] AS Brand_Name,
                   [DevBrand].[Description] AS Brand_Description
            FROM   [dbo].[DeviceModel] AS DevModel
                   INNER JOIN
                   [dbo].[lookup] AS DevBrand
                   ON DevModel.[BrandId] = DevBrand.[code]
            WHERE  DevModel.[Name] = @Name;
        END
END
GO
