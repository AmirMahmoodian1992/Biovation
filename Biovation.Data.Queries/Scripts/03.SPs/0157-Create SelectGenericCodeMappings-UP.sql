
CREATE PROCEDURE [dbo].[SelectGenericCodeMappings]
@manufactureCode nvarchar(50) = 0,
@genericCode nvarchar(50) = 0,
@categoryId int = 0,
@brandCode int = 0
AS
BEGIN
    SELECT [GCM].[Id]
      ,[GCM].[ManufactureCode]
      ,[GCM].[Description]

	  ,[GCL].[Code] AS GenericValue_Code
      ,[GCL].[Name] AS GenericValue_Name
      ,[GCL].[OrderIndex] AS GenericValue_OrderIndex
      ,[GCL].[Description] AS GenericValue_Description

	  ,[GCC].[Id] AS Category_Id
      ,[GCC].[Name] AS Category_Name
      ,[GCC].[Description] AS Category_Description

	  ,[DBL].[Code] AS Brand_Code
      ,[DBL].[Name] AS Brand_Name
      ,[DBL].[OrderIndex] AS Brand_OrderIndex
      ,[DBL].[Description] AS Brand_Description

  FROM [dbo].[GenericCodeMapping] AS [GCM]
    JOIN [dbo].[GenericCodeMappingCategory] AS [GCC] ON [GCM].[CategoryId] = [GCC].[Id]
    JOIN [dbo].[Lookup] AS [GCL] ON [GCM].[GenericCode] = [GCL].[Code]
    LEFT JOIN [dbo].[Lookup] AS [DBL] ON [GCM].[BrandCode] = [DBL].[Code]
    WHERE   ([GCM].[ManufactureCode] = @manufactureCode OR ISNULL(@manufactureCode, 0) = 0) 
		AND ([GCM].[GenericCode] = @genericCode OR ISNULL(@genericCode, 0) = 0)
		AND ([GCM].[CategoryId] = @categoryId OR ISNULL(@categoryId, 0) = 0)
		AND ([GCM].[BrandCode] = @brandCode OR ISNULL(@brandCode, 0) = 0)

END
