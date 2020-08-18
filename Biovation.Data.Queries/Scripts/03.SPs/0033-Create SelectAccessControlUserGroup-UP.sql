
CREATE PROCEDURE [dbo].[SelectAccessControlUserGroup]
@AccessControlId int
AS
BEGIN
    SELECT 
	    ug.Id , ug.[Name] , a.Id as 'AccessGroupId'
    FROM   [dbo].[UserGroup] ug 
	inner join [dbo].AccessGroupUser ag on ag.UserGroupId = ug.Id
	inner join [dbo].[AccessGroup] a on a.ID = ag.AccessGroupId
	where a.ID = @AccessControlId
END
GO
