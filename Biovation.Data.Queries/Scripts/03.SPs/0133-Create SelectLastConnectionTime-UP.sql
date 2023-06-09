
CREATE PROCEDURE [dbo].[SelectLastConnectionTime]
	@deviceId int
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (15) = N'200';

	--SELECT [UpdateDate] FROM [dbo].[DeviceNetworkHistory]
	--	WHERE [DeviceId] = deviceId
	SELECT distinct MAX([CreateAt]) 
	,@Message AS e_Message,
           @Validate AS e_Validate,
           @Code AS e_Code 

	FROM [dbo].[log]
	where [DeviceId] = deviceId and [EventID] = 16001
END
GO
