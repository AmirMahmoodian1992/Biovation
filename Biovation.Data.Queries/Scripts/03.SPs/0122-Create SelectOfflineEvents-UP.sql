
CREATE PROCEDURE [dbo].[SelectOfflineEvents]
	@DeviceCode int
AS
BEGIN

DECLARE	@queryText nvarchar(MAX)



SET @queryText = 'SELECT [Id]
      ,[DeviceCode]
      ,[Data]
      ,[Type]
  FROM [dbo].[OfflineEvents]
  ' + 
  CASE WHEN @DeviceCode = 0 THEN ''
		ELSE 'WHERE [DeviceCode] = ' + CAST(@DeviceCode AS nvarchar)
		END

  EXEC(@queryText)
END
GO
