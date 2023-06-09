CREATE PROCEDURE [dbo].[SelectDeviceModelsByFilter]
@Id INT=NULL,@DeviceBrandId INT=NULL,@Name NVARCHAR (100)=NULL,@PageNumber AS INT=NULL,@PageSize AS INT =NULL
AS
  DECLARE  @HasPaging   BIT;
  DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (15) = N'200';
   
BEGIN


 SET @HasPaging = CASE
                                 WHEN @PageSize =0
                                      AND @PageNumber =0 THEN
                                     0  
                                 ELSE
                                     1
                             END;
 IF @HasPaging = 1
     BEGIN
    SELECT [DevModel].[Id] AS Data_Id,
           [DevModel].[Name] AS Data_Name,
           [DevModel].[ManufactureCode] AS Data_ManufactureCode,
           [DevModel].[GetLogMethodType] AS Data_GetLogMethodType,
           [DevModel].[Description] AS Data_Description,
           [DevModel].[DefaultPortNumber] AS Data_DefaultPortNumber,
           [DevBrand].[code] AS Data_Brand_code,
           [DevBrand].[Name] AS Data_Brand_Name,
           [DevBrand].[Description] AS Data_Brand_Description,
           (@PageNumber-1)*@PageSize  AS [from],
		   @PageNumber AS PageNumber,
		   @PageSize As PageSize,
		   count(*) OVER() AS [Count],
           @Message AS e_Message,
		   @Code  AS e_Code,
		   @Validate  AS e_Validate
    FROM   [dbo].[DeviceModel] AS DevModel
           INNER JOIN
           [dbo].[lookup] AS DevBrand
           ON DevModel.BrandId = DevBrand.code
		 where([DevModel].[Id] = @Id
                OR ISNULL(@Id, 0) = 0)
				AND ([DevModel].[Name] LIKE '%' + @Name + '%'
				 OR ISNULL( @Name, '') = '')
				AND ([DevBrand].[code] = @DeviceBrandId
                OR ISNULL(@DeviceBrandId, 0) = 0)
			ORDER BY DevModel.Id
           OFFSET (@PageNumber-1)*@PageSize ROWS
           FETCH NEXT @PageSize ROWS ONLY
END
Else
   BEGIN
    SELECT [DevModel].[Id] AS Data_Id,
           [DevModel].[Name] AS Data_Name,
           [DevModel].[ManufactureCode] AS Data_ManufactureCode,
           [DevModel].[GetLogMethodType] AS Data_GetLogMethodType,
           [DevModel].[Description] AS Data_Description,
           [DevModel].[DefaultPortNumber] AS Data_DefaultPortNumber,
           [DevBrand].[code] AS Data_Brand_code,
           [DevBrand].[Name] AS Data_Brand_Name,
           [DevBrand].[Description] AS Data_Brand_Description,
           1  AS [from],
		   1 AS PageNumber,
		   count(*) OVER() As PageSize,
		   count(*) OVER() AS [Count],
           @Message AS e_Message,
		   @Code  AS e_Code,
		   @Validate  AS e_Validate
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
END