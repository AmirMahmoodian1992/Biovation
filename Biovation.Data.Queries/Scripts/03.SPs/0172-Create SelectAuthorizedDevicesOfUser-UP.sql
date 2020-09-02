CREATE  PROCEDURE SelectAuthorizedDevicesOfUser 
@UserId int
AS
DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS nvarchar(5) = N'200';   
BEGIN
	SELECT D.[Id] AS Data_DeviceId
      ,D.[Code] AS Data_Code
      ,D.[DeviceModelId] AS Data_DeviceModelId
      ,D.[Name] AS Data_Name
      ,D.[Active] AS Data_Active
      ,D.[IPAddress] AS Data_IPAddress
      ,D.[Port] AS Data_Port
      ,D.[MacAddress] AS Data_MacAddress
      ,D.[RegisterDate] AS Data_RegisterDate
      ,D.[HardwareVersion] AS Data_HardwareVersion
      ,D.[FirmwareVersion] AS Data_FirmwareVersion
      ,D.[DeviceLockPassword] AS Data_DeviceLockPassword
      ,D.[SSL] AS Data_SSL
      ,D.[TimeSync] AS Data_TimeSync
      ,D.[SerialNumber] AS Data_SerialNumber
      ,D.[DeviceTypeId] AS Data_DeviceTypeId
      ,D.[RestaurantId] AS Data_RestaurantId
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
