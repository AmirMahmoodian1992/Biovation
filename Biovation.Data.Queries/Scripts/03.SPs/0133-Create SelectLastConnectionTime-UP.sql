
CREATE PROCEDURE [dbo].[SelectLastConnectionTime]
	@deviceId int
AS
BEGIN

	--SELECT [UpdateDate] FROM [dbo].[DeviceNetworkHistory]
	--	WHERE [DeviceId] = deviceId
	SELECT distinct MAX([CreateAt])  FROM [dbo].[log]
	where [DeviceId] = deviceId and [EventID] = 16001
END
GO
