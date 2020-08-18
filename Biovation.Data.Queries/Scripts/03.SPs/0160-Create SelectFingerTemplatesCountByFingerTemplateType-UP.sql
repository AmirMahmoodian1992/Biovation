create PROCEDURE [dbo].[SelectFingerTemplatesCountByFingerTemplateType]
	@FingerTemplateType NVARCHAR(50)
AS
BEGIN

		SELECT COUNT(*)
		FROM   [dbo].[FingerTemplate] AS [FT]
			   LEFT OUTER JOIN
			   [dbo].[Lookup] AS [LU]
			   ON [FT].[FingerIndex] = [LU].[Code]
			   LEFT OUTER JOIN
			   [dbo].[Lookup] AS [FTLU]
			   ON [FT].[FingerTemplateType] = [FTLU].[Code]
		WHERE  [FT].[FingerTemplateType] = @FingerTemplateType
END