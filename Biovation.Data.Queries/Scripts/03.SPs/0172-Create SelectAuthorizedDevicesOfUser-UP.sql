CREATE PROCEDURE SelectAuthorizedDevicesOfUser
	@UserId int
AS
BEGIN
	SELECT D.[Id] AS DeviceId
      ,D.[Code]
      ,D.[DeviceModelId]
      ,D.[Name]
      ,D.[Active]
      ,D.[IPAddress]
      ,D.[Port]
      ,D.[MacAddress]
      ,D.[RegisterDate]
      ,D.[HardwareVersion]
      ,D.[FirmwareVersion]
      ,D.[DeviceLockPassword]
      ,D.[SSL]
      ,D.[TimeSync]
      ,D.[SerialNumber]
      ,D.[DeviceTypeId]
      ,D.[RestaurantId]
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
