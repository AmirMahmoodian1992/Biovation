
CREATE PROCEDURE [dbo].[SelectFaceTemplatesByUserIdAndIndex]
@UserId bigint, 
@Index INT
AS
BEGIN
    SELECT [FT].[Id],
           [FT].[UserId],
           [FT].[Template],
           [FT].[Index],
           [FT].[CheckSum],
           [FT].[SecurityLevel],
           [FT].[EnrollQuality],
           [FT].[Size],

		   [FTLU].[Code] AS FaceTemplateType_Code,
           [FTLU].[LookupCategoryId] AS FaceTemplateType_LookupCategoryId,
           [FTLU].[Name] AS FaceTemplateType_Name,
           [FTLU].[OrderIndex] AS FaceTemplateType_OrderIndex,
           [FTLU].[Description] AS FaceTemplateType_Description
    FROM   [dbo].[FaceTemplate] AS [FT]
	LEFT OUTER JOIN
		   [dbo].[Lookup] AS [FTLU]
		   ON [FT].[FaceTemplateType] =[FTLU].[Code]
    WHERE  UserId = @UserId
           AND [Index]= @Index
END
GO
