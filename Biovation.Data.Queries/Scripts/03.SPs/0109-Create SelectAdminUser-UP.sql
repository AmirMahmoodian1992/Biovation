Create PROCEDURE [dbo].[SelectAdminUser]
@adminUserId INT = null
AS
BEGIN
	select distinct
	u.Id as Id,
	CAST( u.Id as nvarchar(20)) +'_'+  isnull(u.FirstName,'') +' '+ isnull(u.SurName,'') as FullName    
			  FROM [dbo].[User] U
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
			WHERE u.IsAdmin=1 
            AND (ISNULL(@adminUserId, 0) = 0 OR
            AAG.UserId = @adminUserId
            OR EXISTS (SELECT Id
                       FROM   [dbo].[USER]
                       WHERE  IsMasterAdmin = 1
                              AND ID = @adminUserId))
END