CREATE PROCEDURE [dbo].[SelectFingerTemplatesCountByFingerTemplateType]	
	@FingerTemplateType NVARCHAR(50)
AS
  DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (15) = N'200';
  
BEGIN

		SELECT COUNT(*),
		   @Message AS [Message],         
           @Validate AS Validate,
           @Code AS Code
		FROM   [dbo].[FingerTemplate] AS [FT]
			   LEFT OUTER JOIN
			   [dbo].[Lookup] AS [LU]
			   ON [FT].[FingerIndex] = [LU].[Code]
			   LEFT OUTER JOIN
			   [dbo].[Lookup] AS [FTLU]
			   ON [FT].[FingerTemplateType] = [FTLU].[Code]
		WHERE  [FT].[FingerTemplateType] = @FingerTemplateType
END