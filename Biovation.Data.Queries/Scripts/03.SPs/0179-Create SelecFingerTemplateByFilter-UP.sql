create PROCEDURE [dbo].[SelectFingerTemplateByFilter]
@UserId BIGINT = NULL,@TemplateIndex INT  = NULL,@FingerTemplateType NVARCHAR (50) =NULL,@from INT=NULL, @size INT=NULL
,@PageNumber AS INT=0, @PageSize AS INT =0
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
    IF (@size IS NULL
        OR @size = 0)
        SELECT @size = COUNT(*)
        FROM   [dbo].[FingerTemplate] AS [FT]
               LEFT OUTER JOIN
               [dbo].[Lookup] AS [LU]
               ON [FT].[FingerIndex] = [LU].[Code]
               LEFT OUTER JOIN
               [dbo].[Lookup] AS [FTLU]
               ON [FT].[FingerTemplateType] = [FTLU].[Code]
        WHERE  ([FT].[FingerTemplateType]= @FingerTemplateType
                OR ISNULL(@FingerTemplateType, 0) = 0)
				 and ( ([FT].[UserId] =@UserId
				 OR ISNULL(@UserId, 0) = 0 )and([FT].[TemplateIndex] = @TemplateIndex OR ISNULL(@TemplateIndex, 0) = 0));

    WITH   T
    AS     (SELECT DENSE_RANK() OVER (ORDER BY [FT].[UserId]) AS RowNum,
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
            WHERE  ([FT].[FingerTemplateType]= @FingerTemplateType
                  OR ISNULL(@FingerTemplateType, 0) = 0)
				  AND ( ([FT].[UserId] =@UserId
				  OR ISNULL(@UserId, 0) = 0) AND([FT].[TemplateIndex] = @TemplateIndex OR ISNULL(@TemplateIndex, 0) = 0)))
				
    SELECT *
    FROM   T
    WHERE  	
	RowNum >= ISNULL(@from, 1)
           AND RowNum <= ISNULL(@from, 0) + ISNULL(@size, 0);
END
ELSE
BEGIN
    IF (@size IS NULL
        OR @size = 0)
        SELECT @size = COUNT(*)
        FROM   [dbo].[FingerTemplate] AS [FT]
               LEFT OUTER JOIN
               [dbo].[Lookup] AS [LU]
               ON [FT].[FingerIndex] = [LU].[Code]
               LEFT OUTER JOIN
               [dbo].[Lookup] AS [FTLU]
               ON [FT].[FingerTemplateType] = [FTLU].[Code]
        WHERE ([FT].[FingerTemplateType]= @FingerTemplateType
                 OR ISNULL(@FingerTemplateType, 0) = 0)
				 AND ([FT].[UserId] =@UserId
				 OR ISNULL(@UserId, 0) = 0 AND([FT].[TemplateIndex] = @TemplateIndex OR ISNULL(@TemplateIndex, 0) = 0));				
    WITH   T
    AS     (SELECT DENSE_RANK() OVER (ORDER BY [FT].[UserId]) AS RowNum,
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
            WHERE  ([FT].[FingerTemplateType]= @FingerTemplateType
                OR ISNULL(@FingerTemplateType, 0) = 0)
				 AND (([FT].[UserId] =@UserId
				 OR ISNULL(@UserId, 0) = 0) AND([FT].[TemplateIndex] = @TemplateIndex OR ISNULL(@TemplateIndex, 0) = 0)))
	SELECT *
    FROM   T
    WHERE  	
	RowNum >= ISNULL(@from, 1)
           AND RowNum <= ISNULL(@from, 0) + ISNULL(@size, 0);
END
END