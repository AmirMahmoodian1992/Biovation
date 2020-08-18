IF OBJECT_ID('GetFingerTemplateTypes', 'P') IS NOT NULL
	DROP PROC [dbo].[GetFingerTemplateTypes]
GO


CREATE PROCEDURE [dbo].[SelectFingerTemplateTypes]
@brandId NVARCHAR(50)
AS
BEGIN
   SELECT [LU].[Code]
      ,[LU].[Name]
      ,[LU].[OrderIndex]
      ,[LU].[Description]
	  ,[LU].[LookupCategoryId]
	  ,[GM].[Id] 
	  ,[GM].[Description]
	  ,[GM].[BrandCode]
  FROM [dbo].[Lookup] AS [LU]
	LEFT JOIN
	[dbo].[GenericCodeMapping] AS [GM] ON [LU].[Code] = [GM].[GenericCode]
    WHERE (ISNULL(@brandId, 0) = 0 OR [GM].[BrandCode] = @brandId)
	and [GM].[CategoryId]=9
END
GO
