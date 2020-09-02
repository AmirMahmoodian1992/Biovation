Create PROCEDURE [dbo].[SelectLookups]
@code NVARCHAR (100)=NULL, @name NVARCHAR (100)=NULL, @lookupCategoryId INT=0, @codePrefix NVARCHAR (100)=NULL,@PageNumber AS INT=0, @PageSize AS INT =0
AS
          DECLARE  @HasPaging   BIT;
		  DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @Code_Result AS nvarchar(5) = N'200';
   
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
           [LC].[Id] AS Data_Category_Id,
           [LC].[Name] AS Data_Category_Name,
           [LC].[Prefix] AS Data_Category_Prefix,
           [LC].[Description] AS Data_Category_Description,
		   (@PageNumber-1)*@PageSize  AS [from],
		   @PageNumber AS PageNumber,
		   @PageSize As PageSize,
		   count(*) OVER() AS [Count],
		   @Message AS e_Message,
		   @Code_Result  AS e_Code,
		   @Validate  AS e_Validate
    FROM   [dbo].[Lookup] AS [LU]
           INNER JOIN
           [dbo].[LookupCategory] AS [LC]
           ON [LU].[LookupCategoryId] = [LC].[Id]
    WHERE  ([LU].[Code] = @code
            OR ISNULL(@code, 0) = 0)
           AND ([LU].[Name] = @name
                OR ISNULL(@name, 0) = 0)
           AND ([LC].[Prefix] = @codePrefix
                OR ISNULL(@codePrefix, 0) = 0)
           AND ([LC].[Id] = @lookupCategoryId
                OR ISNULL(@lookupCategoryId, 0) = 0)
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
           [LC].[Id] AS Data_Category_Id,
           [LC].[Name] AS Data_Category_Name,
           [LC].[Prefix] AS Data_Category_Prefix,
           [LC].[Description] AS Data_Category_Description,
		   1 AS [from],
		   1 AS PageNumber,
		   count(*) OVER() As PageSize,
		   count(*) OVER() AS [Count],
		   @Message AS e_Message,
		   @Code_Result  AS e_Code,
		   @Validate  AS e_Validate
    FROM   [dbo].[Lookup] AS [LU]
           INNER JOIN
           [dbo].[LookupCategory] AS [LC]
           ON [LU].[LookupCategoryId] = [LC].[Id]
    WHERE  ([LU].[Code] = @code
            OR ISNULL(@code, 0) = 0)
           AND ([LU].[Name] = @name
                OR ISNULL(@name, 0) = 0)
           AND ([LC].[Prefix] = @codePrefix
                OR ISNULL(@codePrefix, 0) = 0)
           AND ([LC].[Id] = @lookupCategoryId
                OR ISNULL(@lookupCategoryId, 0) = 0);
END
END