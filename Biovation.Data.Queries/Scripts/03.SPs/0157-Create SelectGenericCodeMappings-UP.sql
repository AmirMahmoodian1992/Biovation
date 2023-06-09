
CREATE PROCEDURE [dbo].[SelectGenericCodeMappings]
@manufactureCode nvarchar(50) = 0,
@genericCode nvarchar(50) = 0,
@categoryId int = 0,
@brandCode int = 0,
@PageNumber AS INT=0, @PageSize AS INT =0
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
    SELECT [GCM].[Id] AS Data_Id
      ,[GCM].[ManufactureCode] AS Data_ManufactureCode
      ,[GCM].[Description] AS Data_ManufactureCode

	  ,[GCL].[Code] AS Data_GenericValue_Code
      ,[GCL].[Name] AS Data_GenericValue_Name
      ,[GCL].[OrderIndex] AS Data_GenericValue_OrderIndex
      ,[GCL].[Description] AS Data_GenericValue_Description

	  ,[GCC].[Id] AS Data_Category_Id
      ,[GCC].[Name] AS Data_Category_Name
      ,[GCC].[Description] AS Data_Category_Description

	  ,[DBL].[Code] AS Data_Brand_Code
      ,[DBL].[Name] AS Data_Brand_Name
      ,[DBL].[OrderIndex] AS Data_Brand_OrderIndex
      ,[DBL].[Description] AS Data_Brand_Description,
	  	   (@PageNumber-1)*@PageSize  AS [from],
		   @PageNumber AS PageNumber,
		   @PageSize As PageSize,
		   count(*) OVER() AS [Count],
		   @Message AS e_Message,
		   @Code  AS e_Code,
		   @Validate  AS e_Validate
  FROM [dbo].[GenericCodeMapping] AS [GCM]
    JOIN [dbo].[GenericCodeMappingCategory] AS [GCC] ON [GCM].[CategoryId] = [GCC].[Id]
    JOIN [dbo].[Lookup] AS [GCL] ON [GCM].[GenericCode] = [GCL].[Code]
    LEFT JOIN [dbo].[Lookup] AS [DBL] ON [GCM].[BrandCode] = [DBL].[Code]
    WHERE   ([GCM].[ManufactureCode] = @manufactureCode OR ISNULL(@manufactureCode, 0) = 0) 
		AND ([GCM].[GenericCode] = @genericCode OR ISNULL(@genericCode, 0) = 0)
		AND ([GCM].[CategoryId] = @categoryId OR ISNULL(@categoryId, 0) = 0)
		AND ([GCM].[BrandCode] = @brandCode OR ISNULL(@brandCode, 0) = 0)
		   ORDER BY [GCM].[Id]
           OFFSET (@PageNumber-1)*@PageSize ROWS
           FETCH NEXT @PageSize ROWS ONLY	

END
ELSE
BEGIN

     SELECT [GCM].[Id] AS Data_Id
      ,[GCM].[ManufactureCode] AS Data_ManufactureCode
      ,[GCM].[Description] AS Data_ManufactureCode

	  ,[GCL].[Code] AS Data_GenericValue_Code
      ,[GCL].[Name] AS Data_GenericValue_Name
      ,[GCL].[OrderIndex] AS Data_GenericValue_OrderIndex
      ,[GCL].[Description] AS Data_GenericValue_Description

	  ,[GCC].[Id] AS Data_Category_Id
      ,[GCC].[Name] AS Data_Category_Name
      ,[GCC].[Description] AS Data_Category_Description

	  ,[DBL].[Code] AS Data_Brand_Code
      ,[DBL].[Name] AS Data_Brand_Name
      ,[DBL].[OrderIndex] AS Data_Brand_OrderIndex
      ,[DBL].[Description] AS Data_Brand_Description,
	  	   1 AS [from],
		   1 AS PageNumber,
		   count(*) OVER() As PageSize,
		   count(*) OVER() AS [Count],
		   @Message AS e_Message,
		   @Code  AS e_Code,
		   @Validate  AS e_Validate

  FROM [dbo].[GenericCodeMapping] AS [GCM]
    JOIN [dbo].[GenericCodeMappingCategory] AS [GCC] ON [GCM].[CategoryId] = [GCC].[Id]
    JOIN [dbo].[Lookup] AS [GCL] ON [GCM].[GenericCode] = [GCL].[Code]
    LEFT JOIN [dbo].[Lookup] AS [DBL] ON [GCM].[BrandCode] = [DBL].[Code]
    WHERE   ([GCM].[ManufactureCode] = @manufactureCode OR ISNULL(@manufactureCode, 0) = 0) 
		AND ([GCM].[GenericCode] = @genericCode OR ISNULL(@genericCode, 0) = 0)
		AND ([GCM].[CategoryId] = @categoryId OR ISNULL(@categoryId, 0) = 0)
		AND ([GCM].[BrandCode] = @brandCode OR ISNULL(@brandCode, 0) = 0)
END
END
