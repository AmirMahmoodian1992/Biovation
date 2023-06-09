
CREATE PROCEDURE [dbo].[SelectFingerTemplatesByUserIdAndTemplateIndex]
@UserId bigint, @TemplateIndex INT
AS
BEGIN
    SELECT [FT].[Id],
           [FT].[UserId],
           [FT].[TemplateIndex],
           [FT].[Template],
           [FT].[Index],
           [FT].[Duress],
           [FT].[CheckSum],
           [FT].[SecurityLevel],
           [FT].[EnrollQuality],
           [FT].[Size],
		   [FT].[FingerTemplateType],

		   [LU].[Code] AS FingerIndex_Code,
		   [LU].[LookupCategoryId] AS FingerIndex_LookupCategoryId,
		   [LU].[Name] AS FingerIndex_Name,
		   [LU].[OrderIndex] AS FingerIndex_OrderIndex,
		   [LU].[Description] AS FingerIndex_Description,
   		   [FTLU].[Code] AS FingerTemplateType_Code,
           [FTLU].[LookupCategoryId] AS FingerTemplateType_LookupCategoryId,
           [FTLU].[Name] AS FingerTemplateType_Name,
           [FTLU].[OrderIndex] AS FingerTemplateType_OrderIndex,
           [FTLU].[Description] AS FingerTemplateType_Description
    FROM   [dbo].[FingerTemplate] AS [FT]
           LEFT OUTER JOIN
           [dbo].[Lookup] AS [LU]
           ON [FT].[FingerIndex] = [LU].[Code]
		   LEFT OUTER JOIN
		   [dbo].[Lookup] AS [FTLU]
		   ON [FT].[FingerTemplateType] =[FTLU].[Code]
    WHERE  [FT].[UserId] = @UserId
           AND [FT].[TemplateIndex] = @TemplateIndex
END
GO
