
CREATE PROCEDURE [dbo].[SelectDeviceModelById]
@Id INT
AS
BEGIN
    IF EXISTS (SELECT code
               FROM   [lookup]
               WHERE  code = @Id)
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
                   ON DevModel.[BrandId] = DevBrand.[Code]
            WHERE  DevModel.[Id] = @Id;
        END
END
GO
