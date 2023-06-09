CREATE PROCEDURE SelectAuthorizedUsersOfDevice
	@DeviceId INT,@PageNumber AS INT=0, @PageSize AS INT =0
AS
          DECLARE  @HasPaging   BIT;
		  DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (15) = N'200';
  BEGIN  
 SET @HasPaging = CASE
                                 WHEN @PageSize =0
                                      AND @PageNumber =0 THEN
                                     0
                                 ELSE
                                     1
                             END;
 IF @HasPaging = 1
     BEGIN
	SELECT U.[Id] AS Data_Id
      ,U.[FirstName] AS Data_FirstName
      ,U.[SurName] AS Data_SurName
      ,U.[UserName] AS Data_UserName
      ,U.[StartDate] AS Data_StartDate
      ,U.[EndDate] AS Data_EndDate
      ,U.[RegisterDate] AS Data_RegisterDate
      ,U.[IsActive] AS Data_IsActive
      ,U.[IsAdmin] AS Data_IsAdmin
      ,U.[AdminLevel] AS Data_AdminLevel
      ,U.[Password] AS Data_Password
      ,U.[PasswordBytes] AS Data_PasswordBytes
      ,U.[AuthMode] AS Data_AuthMode
      ,U.[IsMasterAdmin] AS Data_IsMasterAdmin
      ,U.[Image] AS Data_Image
      ,U.[EntityId] AS Data_EntityId
      ,U.[Email] AS Data_Email
      ,U.[TelNumber] AS Data_TelNumber
      ,U.[Type] AS Data_Type
      ,U.[RemainingCredit] AS Data_RemainingCredit
      ,U.[AllowedStockCount] AS Data_AllowedStockCount
	  , (@PageNumber-1)*@PageSize  AS [from],
		   @PageNumber AS PageNumber,
		   @PageSize As PageSize,
		   count(*) OVER() AS [Count],
		   @Message AS e_Message,
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
  WHERE D.Id = @DeviceId
  ORDER BY D.Id
           OFFSET (@PageNumber-1)*@PageSize ROWS
           FETCH NEXT @PageSize ROWS ONLY	
END
ELSE
BEGIN
SELECT U.[Id] AS Data_Id
      ,U.[FirstName] AS Data_FirstName
      ,U.[SurName] AS Data_SurName
      ,U.[UserName] AS Data_UserName
      ,U.[StartDate] AS Data_StartDate
      ,U.[EndDate] AS Data_EndDate
      ,U.[RegisterDate] AS Data_RegisterDate
      ,U.[IsActive] AS Data_IsActive
      ,U.[IsAdmin] AS Data_IsAdmin
      ,U.[AdminLevel] AS Data_AdminLevel
      ,U.[Password] AS Data_Password
      ,U.[PasswordBytes] AS Data_PasswordBytes
      ,U.[AuthMode] AS Data_AuthMode
      ,U.[IsMasterAdmin] AS Data_IsMasterAdmin
      ,U.[Image] AS Data_Image
      ,U.[EntityId] AS Data_EntityId
      ,U.[Email] AS Data_Email
      ,U.[TelNumber] AS Data_TelNumber
      ,U.[Type] AS Data_Type
      ,U.[RemainingCredit] AS Data_RemainingCredit
      ,U.[AllowedStockCount] AS Data_AllowedStockCount,
		   1 AS [from],
		   1 AS PageNumber,
		   count(*) OVER() As PageSize,
		   count(*) OVER() AS [Count],
		   @Message AS e_Message,
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
  WHERE D.Id = @DeviceId
END
END
GO
