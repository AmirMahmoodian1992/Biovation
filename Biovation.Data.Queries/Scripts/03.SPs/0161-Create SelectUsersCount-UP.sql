
CREATE PROCEDURE [dbo].[SelectUsersCount]
AS
BEGIN
	SELECT COUNT(*) FROM [dbo].[User]
END
GO
