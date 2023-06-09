
CREATE proc [dbo].[SelectSearchUser]
@FilterText NVARCHAR (100)=NULL,
@adminUserId INT = null
AS
SELECT TOP 20 u.Id AS Id,
              CAST (u.Id AS NVARCHAR (20)) + '_' + isnull(u.FirstName, '') + ' ' + isnull(u.SurName, '') AS FullName
FROM   [User] AS u
LEFT OUTER JOIN
           [dbo].[UserGroupMember] AS UGM
           ON UGM.UserId = u.Id
           LEFT OUTER JOIN
           [dbo].[UserGroup] AS UG
           ON UG.Id = UGM.GroupId
           LEFT OUTER JOIN
           [dbo].[AccessGroupUser] AS AGU
           ON AGU.UserGroupId = UG.Id
           LEFT OUTER JOIN
           [dbo].[AccessGroup] AS AG
           ON AG.Id = AGU.AccessGroupId
		   LEFT OUTER JOIN 
		   [dbo].[AdminAccessGroup] AAG
		   ON AG.Id = AAG.AccessGroupId 
WHERE  (@FilterText IS NULL)
       OR (u.FirstName LIKE '%' + @FilterText + '%')
       OR (u.SurName LIKE '%' + @FilterText + '%')
       OR (u.Id LIKE @FilterText + '%')
	   AND (ISNULL(@adminUserId, 0) = 0 OR
            AAG.UserId = @adminUserId
            OR EXISTS (SELECT Id
                       FROM   [dbo].[USER]
                       WHERE  IsMasterAdmin = 1
                              AND ID = @adminUserId))
 