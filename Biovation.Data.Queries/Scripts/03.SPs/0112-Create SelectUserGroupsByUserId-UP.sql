CREATE PROCEDURE [dbo].[SelectUserGroupsByUserId]
@UserId INT
AS
BEGIN
    SELECT [UG].[Id],
           [UG].[Name],
           [UG].[AccessGroup],
           [UG].[Description],
		   isnull(u.FirstName,'') + ' ' + isnull(u.SurName,'') as Users_UserName,
		   [UGM].[Id] AS Users_Id,
		   [UGM].[UserId] AS Users_UserId,
		   [u].Code AS Users_UserCode,
		   [UGM].[GroupId] AS Users_GroupId,
		   [UGM].[UserType] AS Users_UserType,
		   [UGM].[UserTypeTitle] AS Users_UserTypeTitle
    FROM   [dbo].[UserGroup] AS UG	LEFT OUTER JOIN [dbo].[UserGroupMember] AS UGM ON UGM.[GroupId] = UG.[Id]
									LEFT OUTER JOIN [dbo].[User] u on ugm.[UserId] = u.[ID]
									WHERE [UGM].[UserId] = @UserId
END
GO