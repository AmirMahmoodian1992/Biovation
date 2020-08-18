
CREATE PROCEDURE [dbo].[SelectLookups]
@code nvarchar(100) = NULL,
@name nvarchar(100) = NULL,
@lookupCategoryId int = 0,
@codePrefix nvarchar(100) = NULL
AS
BEGIN
    SELECT [LU].[Code]
      ,[LU].[Name]
      ,[LU].[OrderIndex]
      ,[LU].[Description]

	  ,[LC].[Id] AS Category_Id
      ,[LC].[Name] AS Category_Name
      ,[LC].[Prefix] AS Category_Prefix
      ,[LC].[Description] AS Category_Description

  FROM [dbo].[Lookup] AS [LU]
    JOIN [dbo].[LookupCategory] AS [LC] ON [LU].[LookupCategoryId] = [LC].[Id]
    WHERE   ([LU].[Code] = @code OR ISNULL(@code, 0) = 0) 
		AND ([LU].[Name] = @name OR ISNULL(@name, 0) = 0)
		AND ([LC].[Prefix] = @codePrefix OR ISNULL(@codePrefix, 0) = 0)
		AND ([LC].[Id] = @lookupCategoryId OR ISNULL(@lookupCategoryId, 0) = 0)

END
