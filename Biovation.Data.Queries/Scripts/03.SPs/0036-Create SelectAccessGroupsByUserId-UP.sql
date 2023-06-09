
CREATE PROCEDURE [dbo].[SelectAccessGroupsByUserId]
@UserId int
AS
BEGIN

		SELECT [AG].[ID],
           [AG].[Name],
           [AG].[TimeZoneId],
           [USG].[Id] AS UserGroup_Id,
           [USG].[Name] AS UserGroup_Name,
           [AG].[Description],
           AAG.UserId AS AdminUserId_Id
    FROM   [dbo].[AccessGroup] AS AG
           LEFT OUTER JOIN
           [dbo].[AccessGroupUser] AS AGU
           ON AGU.AccessGroupId = AG.ID
           LEFT OUTER JOIN
           [dbo].[UserGroup] AS USG
           ON AGU.UserGroupId = USG.ID
           LEFT OUTER JOIN
           [dbo].[AdminAccessGroup] AS AAG
           ON AG.Id = AAG.AccessGroupId
		   LEFT OUTER JOIN
		   [dbo].[UserGroupMember] AS UGM
		   ON UGM.GroupId = USG.Id
				WHERE [UGM].UserId = @UserId
END
GO
