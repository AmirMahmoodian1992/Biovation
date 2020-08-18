CREATE PROCEDURE SelectAuthorizedUsersOfDevice
	@DeviceId INT
AS
BEGIN
	SELECT U.[Id]
      ,U.[FirstName]
      ,U.[SurName]
      ,U.[UserName]
      ,U.[StartDate]
      ,U.[EndDate]
      ,U.[RegisterDate]
      ,U.[IsActive]
      ,U.[IsAdmin]
      ,U.[AdminLevel]
      ,U.[Password]
      ,U.[PasswordBytes]
      ,U.[AuthMode]
      ,U.[IsMasterAdmin]
      ,U.[Image]
      ,U.[EntityId]
      ,U.[Email]
      ,U.[TelNumber]
      ,U.[Type]
      ,U.[RemainingCredit]
      ,U.[AllowedStockCount]
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
  WHERE D.Id = @DeviceId
END
GO
