
CREATE PROCEDURE [dbo].[DeleteOfflineEventByValues]
	@deviceCode int,
	@data nvarchar(MAX),
	@type int
AS
BEGIN

DELETE FROM [dbo].[OfflineEvents]
	WHERE [deviceCode] = @deviceCode AND [data] = @data AND [type] = @type

END
GO
