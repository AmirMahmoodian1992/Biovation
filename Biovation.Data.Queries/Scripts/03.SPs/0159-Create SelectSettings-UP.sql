
CREATE PROCEDURE [dbo].[SelectSettings]
@Key nvarchar(100) = NULL
AS
DECLARE @Message AS NVARCHAR (200) = N' درخواست با موفقیت انجام گرفت', @Validate AS INT = 1,  @Code AS NVARCHAR (15) = N'200';
BEGIN
    SELECT [Id]
      ,[Key]
      ,[Value]
      ,[Description]
      ,@Message AS e_Message,
      @Validate AS e_Validate,
      @Code AS e_Code
	FROM [dbo].[Setting]
    WHERE  ([Key] = @Key
           OR ISNULL(@Key, '') = '')           
END
