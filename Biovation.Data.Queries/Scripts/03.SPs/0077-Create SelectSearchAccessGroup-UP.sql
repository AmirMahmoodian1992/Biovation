
CREATE PROCEDURE [dbo].[SelectSearchAccessGroup]
@Id INT, @DeviceGroupId INT, @UserId INT, @adminUserId INT = null
AS
BEGIN
    SELECT [U].[Id]
    INTO   #T1
    FROM   [UserGroup] AS U
           INNER JOIN
           [UserGroupMember] AS UM
           ON [U].[Id] = [UM].[GroupId]
    WHERE  [UM].[UserId] = @UserId
    SELECT [AG].[Id],
           [AG].[Name],
           [AG].[TimeZoneId],
           [AGU].[Id] AS UserGroup_Id,
           [USG].[Name] AS UserGroup_Name,
           [AGD].[Id] AS DeviceGroup_Id,
           [DG].[Name] AS DeviceGroup_Name,
           [AG].[Description]
    FROM   [dbo].[AccessGroup] AS AG
           LEFT OUTER JOIN
           [dbo].[AccessGroupUser] AS AGU
           ON [AGU].[AccessGroupId] = [AG].[Id]
           LEFT OUTER JOIN
           [dbo].[UserGroup] AS USG
           ON [AGU].[UserGroupId] = [USG].[Id]
           LEFT OUTER JOIN
           [dbo].[AccessGroupDevice] AS AGD
           ON [AGD].[AccessGroupId] = [AG].[Id]
           LEFT OUTER JOIN
           [dbo].[DeviceGroup] AS DG
           ON [AGD].[DeviceGroupId] = [DG].[Id]
		   LEFT OUTER JOIN 
		   [dbo].[AdminAccessGroup] AAG
		   ON AG.id = AAG.AccessGroupId
    WHERE  (isnull(@id, 0) = 0
            OR @id = [AG].[Id])
           AND (isnull(@DevicegroupId, 0) = 0
                OR @DeviceGroupId = [AGD].[DeviceGroupId])
           AND (isnull(@UserId, 0) = 0
                OR [AGU].[UserGroupId] IN (SELECT *
                                           FROM   #T1))
		  AND (isnull(@adminUserId, 0) = 0
            OR AAG.UserId = @adminUserId
            OR EXISTS (SELECT Id
                       FROM   [dbo].[USER]
                       WHERE  IsMasterAdmin = 1
                              AND ID = @adminUserId)) 
    DROP TABLE #T1
END
GO
