Create  PROCEDURE [dbo].[SelectDeviceModelsByFilter]
@Id INT=NULL,@DeviceBrandId INT=NULL,@Name NVARCHAR (100)=NULL
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
           ON DevModel.BrandId = DevBrand.code
		   where([DevModel].[Id] = @Id
                OR ISNULL(@Id, 0) = 0)
				AND ([DevModel].[Name] LIKE '%' + @Name + '%'
				 OR ISNULL( @Name, '') = '')
				AND ([DevBrand].[code] = @DeviceBrandId
                OR ISNULL(@DeviceBrandId, 0) = 0);
END
