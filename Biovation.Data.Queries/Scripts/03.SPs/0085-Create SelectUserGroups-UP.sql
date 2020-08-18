
CREATE PROCEDURE [dbo].[SelectUserGroups]
@adminUserId BIGINT = 0
AS
BEGIN
    SELECT [UG].[Id],
           [UG].[Name],
           [UG].[AccessGroup],
           [UG].[Description],
           isnull(u.FirstName, '') + ' ' + isnull(u.SurName, '') AS Users_UserName,
           [UGM].[Id] AS Users_Id,
           [UGM].[UserId] AS Users_UserId,
           [UGM].[GroupId] AS Users_GroupId,
           [UGM].[UserType] AS Users_UserType,
           [UGM].[UserTypeTitle] AS Users_UserTypeTitle
    FROM   [dbo].[UserGroup] AS UG
           LEFT OUTER JOIN
           [dbo].[UserGroupMember] AS UGM
           ON UGM.GroupId = UG.Id
           LEFT OUTER JOIN
           [dbo].[User] AS u
           ON ugm.UserId = u.ID
           LEFT OUTER JOIN
           [dbo].[AccessGroupUser] AS agu
           ON agu.UserGroupId = ug.Id
           LEFT OUTER JOIN
           [dbo].[AccessGroup] AS ag
           ON ag.Id = agu.AccessGroupId
		     LEFT OUTER JOIN
           [dbo].[AdminAccessGroup] AS AAG
           ON AG.Id = AAG.AccessGroupId
    WHERE  (
	ISNULL(@adminUserId, 0) = 0  OR
            AAG.UserId = @AdminUserId
            OR EXISTS (SELECT Id
                       FROM   [dbo].[USER]
                       WHERE  IsMasterAdmin = 1
                              AND ID = @AdminUserId))
END
GO
