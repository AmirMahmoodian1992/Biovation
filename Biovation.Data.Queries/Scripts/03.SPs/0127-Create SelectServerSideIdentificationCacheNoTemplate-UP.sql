
CREATE PROCEDURE [dbo].[SelectServerSideIdentificationCacheNoTemplate]
@adminUserId INT=0
AS
BEGIN
    SELECT ROW_NUMBER() OVER (ORDER BY [AG].[Id] ASC) AS Id,
		   [AG].[Id] AS AccessGroupId,
           [UGM].[UserId] AS UserId,
           [UGM].[UserType] AS UserType,
           [DGM].[DeviceId] AS DeviceId
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
    WHERE  AAG.UserId = @adminUserId
           OR ISNULL(@adminUserId, 0) = 0
           OR EXISTS (SELECT Id
                      FROM   [dbo].[USER]
                      WHERE  IsMasterAdmin = 1
                             AND ID = @AdminUserId)
END