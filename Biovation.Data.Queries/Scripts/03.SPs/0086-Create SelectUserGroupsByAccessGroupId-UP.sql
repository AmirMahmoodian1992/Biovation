CREATE PROCEDURE [dbo].[SelectUserGroupsByAccessGroupId]
@accessGroupId INT
AS
BEGIN
    SELECT [UG].[Id],
           [UG].[Name],
           [UG].[AccessGroup],
           [UG].[Description],
		   [UGM].[Id] AS Users_Id,
		   [UGM].[UserId] AS Users_UserId,
		   [u].Code AS Users_UserCode,
		   [UGM].[GroupId] AS Users_GroupId,
		   [UGM].[UserType] AS Users_UserType,
		   [UGM].[UserTypeTitle] AS Users_UserTypeTitle
    FROM   [dbo].[UserGroup] AS UG LEFT JOIN [dbo].[UserGroupMember] AS UGM ON UGM.GroupId = UG.Id
									LEFT JOIN [dbo].[AccessGroupUser] AS AGU ON AGU.UserGroupId = UG.Id
									LEFT OUTER JOIN [dbo].[User] u on ugm.[UserId] = u.[ID]
	WHERE [AGU].[AccessGroupId] = @accessGroupId
END
GO
