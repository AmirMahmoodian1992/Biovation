Create PROCEDURE [dbo].[SelectDeviceBrandsByFilter]
@Code INT=NULL,@Name NVARCHAR (100)=NULL,@PageNumber AS INT=NULL,@PageSize AS INT =NULL
AS
BEGIN
    DECLARE  @HasPaging  BIT;
	 SET @HasPaging = CASE
                                   WHEN @PageSize =0
                                      AND @PageNumber =0 THEN
                                     0                                   
                                 ELSE
                                     1
                             END;
 IF @HasPaging = 1
  BEGIN
    SELECT DevBrand.[code]  AS Data_code,
           DevBrand.[Name]  AS Data_Name,
           DevBrand.[Description]  AS Data_Description,
           (@PageNumber-1)*@PageSize  AS [from],
		   @PageNumber AS PageNumber,
		   @PageSize As PageSize,
		   count(*) OVER() AS [Count]
    FROM   [dbo].[Lookup] AS DevBrand
    WHERE  DevBrand.LookupCategoryId = 6
					AND (DevBrand.[Code] = @Code
                OR ISNULL(@Code, 0) = 0)
					AND (DevBrand.[Name] LIKE '%' + @Name + '%'
				 OR ISNULL( @Name, '') = '')
			ORDER BY DevBrand.[code]
           OFFSET (@PageNumber-1)*@PageSize ROWS
           FETCH NEXT @PageSize ROWS ONLY
  END
Else
   BEGIN
    SELECT DevBrand.[code]  AS Data_code,
           DevBrand.[Name]  AS Data_Name,
           DevBrand.[Description]  AS Data_Description,
           1  AS [from],
		   1 AS PageNumber,
		   count(*) OVER() As PageSize,
		   count(*) OVER() AS [Count]
    FROM   [dbo].[Lookup] AS DevBrand
    WHERE  DevBrand.LookupCategoryId = 6
					AND (DevBrand.[Code] = @Code
                OR ISNULL(@Code, 0) = 0)
					AND (DevBrand.[Name] LIKE '%' + @Name + '%'
				 OR ISNULL( @Name, '') = '');
END
END