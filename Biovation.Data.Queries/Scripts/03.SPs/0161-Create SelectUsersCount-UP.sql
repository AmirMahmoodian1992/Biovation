CREATE PROCEDURE [dbo].[SelectUsersCount]
AS
DECLARE @Message AS NVARCHAR (200) = N' درخواست با موفقیت انجام گرفت', @Validate AS INT = 1,  @Code AS NVARCHAR (15) = N'200';
BEGIN
	SELECT COUNT(*) AS Data_Data,@Message AS e_Message,
           @Validate AS e_Validate,
           @Code AS e_Code FROM [dbo].[User]	       
		   
END
GO
