create PROCEDURE [dbo].[SelectFingerTemplatesByFingerTemplateType]
	@FingerTemplateType NVARCHAR(50),
	@from int = NULL,
	@size int = NULL
AS
BEGIN
    IF (@size IS NULL OR @size = 0)
		SELECT @size = COUNT(*) FROM [dbo].[FingerTemplate] AS [FT]
			   LEFT OUTER JOIN
			   [dbo].[Lookup] AS [LU]
			   ON [FT].[FingerIndex] = [LU].[Code]
			   LEFT OUTER JOIN
			   [dbo].[Lookup] AS [FTLU]
			   ON [FT].[FingerTemplateType] = [FTLU].[Code]
		WHERE  [FT].[FingerTemplateType] = @FingerTemplateType

	;WITH T AS (SELECT DENSE_RANK() OVER ( Order BY [FT].[UserId] ) AS RowNum,
			   [FT].[Id],
			   [FT].[UserId],
			   [FT].[TemplateIndex],
			   [FT].[Template],
			   [FT].[Index],
			   [FT].[Duress],
			   [FT].[CheckSum],
			   [FT].[SecurityLevel],
			   [FT].[EnrollQuality],
			   [FT].[Size],
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
			   ON [FT].[FingerTemplateType] = [FTLU].[Code]
		WHERE  [FT].[FingerTemplateType] = @FingerTemplateType)

	SELECT * FROM T
		
	WHERE RowNum >= ISNULL(@from, 1)
	  AND RowNum <= ISNULL(@from, 0) + ISNULL(@size, 0)
	  --AND UserId <> ISNULL((SELECT TOP (1) UserId From T WHERE RowNum = ISNULL(@from, 0) + ISNULL(@size, 0)), 0)
END