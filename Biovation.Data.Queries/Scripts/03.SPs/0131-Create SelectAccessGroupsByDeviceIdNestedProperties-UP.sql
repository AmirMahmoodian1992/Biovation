
CREATE PROCEDURE [dbo].[SelectAccessGroupsByDeviceIdNestedProperties]
@DeviceId int
AS
BEGIN

		SELECT [AG].[ID],
           [AG].[Name],
           [AG].[TimeZoneId],
           [USG].[Id] AS UserGroup_Id,
           [USG].[Name] AS UserGroup_Name,
           [DG].[Id] AS DeviceGroup_Id,
           [DG].[Name] AS DeviceGroup_Name,
           [AG].[Description],
           AAG.UserId AS AdminUserId_Id,
		   [UGM].[UserId] AS UserGroup_Users_UserId,
		   [UGM].[UserType] AS UserGroup_Users_UserType,
		   [UGM].[UserTypeTitle] AS UserGroup_Users_UserTypeTitle,
		   [DGM].[DeviceId] AS DeviceGroup_Devices_DeviceId
    FROM   [dbo].[AccessGroup] AS AG
           LEFT OUTER JOIN
           [dbo].[AccessGroupUser] AS AGU
           ON AGU.AccessGroupId = AG.ID
           LEFT OUTER JOIN
           [dbo].[UserGroup] AS USG
           ON AGU.UserGroupId = USG.ID
           LEFT OUTER JOIN
           [dbo].[AccessGroupDevice] AS AGD
           ON AGD.AccessGroupId = AG.Id
           LEFT OUTER JOIN
           [dbo].[DeviceGroup] AS DG
           ON AGD.DeviceGroupId = DG.ID
           LEFT OUTER JOIN
           [dbo].[AdminAccessGroup] AS AAG
           ON AG.Id = AAG.AccessGroupId
		   LEFT OUTER JOIN
		   [dbo].[UserGroupMember] AS UGM
		   ON UGM.GroupId = USG.Id
		   LEFT OUTER JOIN
		   [dbo].[DeviceGroupMember] AS DGM
		   ON DGM.GroupId = DG.ID
				WHERE [DGM].DeviceId = @DeviceId
END
GO
