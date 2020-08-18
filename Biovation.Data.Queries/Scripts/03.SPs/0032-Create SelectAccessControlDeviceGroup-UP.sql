
CREATE PROCEDURE [dbo].[SelectAccessControlDeviceGroup]
@AccessControlId int
AS
BEGIN
    SELECT 
	    ug.Id , ug.[Name] , a.Id as 'AccessGroupId'
    FROM   [dbo].[DeviceGroup] ug 
	inner join [dbo].AccessGroupDevice ag on ag.DeviceGroupId = ug.Id
	inner join [dbo].[AccessGroup] a on a.ID = ag.AccessGroupId
	where a.ID = @AccessControlId
END
GO
