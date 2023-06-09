
CREATE PROCEDURE [dbo].[SelectAccessGroupsByDeviceId]
@DeviceId int
AS
BEGIN

		SELECT [AG].[ID],
           [AG].[Name],
           [AG].[TimeZoneId],
           [DG].[Id] AS DeviceGroup_Id,
           [DG].[Name] AS DeviceGroup_Name,
           [AG].[Description],
           AAG.UserId AS AdminUserId_Id
    FROM   [dbo].[AccessGroup] AS AG
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
		   [dbo].[DeviceGroupMember] AS DGM
		   ON DGM.GroupId = DG.ID
				WHERE [DGM].DeviceId = @DeviceId
END
GO
