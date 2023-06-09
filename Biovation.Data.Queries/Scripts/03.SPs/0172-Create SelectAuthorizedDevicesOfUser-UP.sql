CREATE  PROCEDURE SelectAuthorizedDevicesOfUser 
@UserId int
AS
DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (15) = N'200';   
BEGIN
	SELECT D.[Id] AS DeviceId
      ,D.[Code] AS Code
      ,D.[DeviceModelId] AS DeviceModelId
      ,D.[Name] AS Name
      ,D.[Active] AS Active
      ,D.[IPAddress] AS IPAddress
      ,D.[Port] AS Port
      ,D.[MacAddress] AS MacAddress
      ,D.[RegisterDate] AS RegisterDate
      ,D.[HardwareVersion] AS HardwareVersion
      ,D.[FirmwareVersion] AS FirmwareVersion
      ,D.[DeviceLockPassword] AS DeviceLockPassword
      ,D.[SSL] AS SSL
      ,D.[TimeSync] AS TimeSync
      ,D.[SerialNumber] AS SerialNumber
      ,D.[DeviceTypeId] AS DeviceTypeId
      ,D.[RestaurantId] AS RestaurantId
	  ,@Message AS e_Message,
	   @Code  AS e_Code,
	   @Validate  AS e_Validate
  FROM [dbo].[User] AS U
  JOIN [dbo].[UserGroupMember] AS UGM
	ON U.Id = UGM.UserId
  JOIN [dbo].[AccessGroupUser] AS AGU
    ON UGM.GroupId = AGU.UserGroupId
  JOIN [dbo].[AccessGroup] AS AG
    ON AGU.AccessGroupId = AG.Id
  JOIN [dbo].[AccessGroupDevice] AS AGD
    ON AG.Id = AGD.AccessGroupId
  JOIN [dbo].[DeviceGroupMember] AS DGM
    ON AGD.DeviceGroupId = DGM.GroupId
  JOIN [dbo].[Device] AS D
    ON DGM.DeviceId = D.Id
  WHERE U.Id = @UserId
END
GO
