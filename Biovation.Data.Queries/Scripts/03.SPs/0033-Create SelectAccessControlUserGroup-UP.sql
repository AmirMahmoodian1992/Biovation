
CREATE PROCEDURE [dbo].[SelectAccessControlUserGroup]

@AccessControlId int
AS
DECLARE @Message AS NVARCHAR (200) = N' درخواست با موفقیت انجام گرفت', @Validate AS INT = 1,  @Code AS NVARCHAR (15) = N'200';

BEGIN
    SELECT 
	    ug.Id , ug.[Name] , a.Id as 'AccessGroupId',
		   @Message AS e_Message,
           @Validate AS e_Validate,
           @Code AS e_Code
    FROM   [dbo].[UserGroup] ug 
	inner join [dbo].AccessGroupUser ag on ag.UserGroupId = ug.Id
	inner join [dbo].[AccessGroup] a on a.ID = ag.AccessGroupId
	where a.ID = @AccessControlId
END
GO
