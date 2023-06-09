IF OBJECT_ID('GetFingerTemplateTypes', 'P') IS NOT NULL
	DROP PROC [dbo].[GetFingerTemplateTypes]
GO


CREATE PROCEDURE [dbo].[SelectFingerTemplateTypes]
@brandId NVARCHAR (50),@PageNumber AS INT=0, @PageSize AS INT =0
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
    SELECT [LU].[Code] AS Data_Code,
           [LU].[Name] AS Data_Name,
           [LU].[OrderIndex] AS Data_OrderIndex,
           [LU].[Description] AS Data_Description,
           [LU].[LookupCategoryId] AS Data_LookupCategoryId,
		   (@PageNumber-1)*@PageSize  AS [from],
		   @PageNumber AS PageNumber,
		   @PageSize As PageSize,
		   count(*) OVER() AS [Count],
		   @Message AS e_Message,
		   @Code  AS e_Code,
		   @Validate  AS e_Validate
    FROM   [dbo].[Lookup] AS [LU]
           LEFT OUTER JOIN
           [dbo].[GenericCodeMapping] AS [GM]
           ON [LU].[Code] = [GM].[GenericCode]
    WHERE  (ISNULL(@brandId, 0) = 0
            OR [GM].[BrandCode] = @brandId)
           AND [GM].[CategoryId] = 9
		   ORDER BY [LU].[Code]
           OFFSET (@PageNumber-1)*@PageSize ROWS
           FETCH NEXT @PageSize ROWS ONLY	
END
ELSE
BEGIN
    SELECT [LU].[Code] AS Data_Code,
           [LU].[Name] AS Data_Name,
           [LU].[OrderIndex] AS Data_OrderIndex,
           [LU].[Description] AS Data_Description,
           [LU].[LookupCategoryId] AS Data_LookupCategoryId,
		   1 AS [from],
		   1 AS PageNumber,
		   count(*) OVER() As PageSize,
		   count(*) OVER() AS [Count],
		   @Message AS e_Message,
		   @Code  AS e_Code,
		   @Validate  AS e_Validate
    FROM   [dbo].[Lookup] AS [LU]
           LEFT OUTER JOIN
           [dbo].[GenericCodeMapping] AS [GM]
           ON [LU].[Code] = [GM].[GenericCode]
    WHERE  (ISNULL(@brandId, 0) = 0
            OR [GM].[BrandCode] = @brandId)
           AND [GM].[CategoryId] = 9;
END
END
GO
