Create PROCEDURE [dbo].[SelectAccessGroupsByFilter]
@adminUserId INT=0,@Id INT=0,@DeviceId INT=0,@UserGroupId INT=0,@UserId INT=0
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
           [AAG].UserId AS AdminUserId_Id,
           [UGM].[UserId] AS UserGroup_Users_UserId,
           [UGM].[UserType] AS UserGroup_Users_UserType,
           [UGM].[UserTypeTitle] AS UserGroup_Users_UserTypeTitle,
           [DGM].[DeviceId] AS DeviceGroup_Devices_DeviceId,
           [TZ].[Id] AS TimeZone_Id,
           [TZ].[Name] AS TimeZone_Name,
           [TZD].[Id] AS TimeZone_Details_Id,
           [TZD].[TimeZoneId] AS TimeZone_Details_TimeZoneId,
           [TZD].[DayNumber] AS TimeZone_Details_DayNumber,
           [TZD].[FromTime] AS TimeZone_Details_FromTime,
           [TZD].[ToTime] AS TimeZone_Details_ToTime
    FROM   [dbo].[AccessGroup] AS [AG]
           LEFT OUTER JOIN
           [dbo].[AccessGroupUser] AS [AGU]
           ON AGU.AccessGroupId = AG.ID
           LEFT OUTER JOIN
           [dbo].[UserGroup] AS [USG]
           ON AGU.UserGroupId = USG.ID
           LEFT OUTER JOIN
           [dbo].[AccessGroupDevice] AS [AGD]
           ON AGD.AccessGroupId = AG.Id
           LEFT OUTER JOIN
           [dbo].[DeviceGroup] AS [DG]
           ON AGD.DeviceGroupId = DG.ID
           LEFT OUTER JOIN
           [dbo].[AdminAccessGroup] AS [AAG]
           ON AG.Id = AAG.AccessGroupId
           LEFT OUTER JOIN
           [dbo].[UserGroupMember] AS [UGM]
           ON UGM.GroupId = USG.Id
           LEFT OUTER JOIN
           [dbo].[DeviceGroupMember] AS [DGM]
           ON DGM.GroupId = DG.ID
           LEFT OUTER JOIN
           [dbo].[TimeZone] AS [TZ]
           ON [AG].[TimeZoneId] = [TZ].[Id]
           LEFT OUTER JOIN
           [dbo].[TimeZoneDetail] AS TZD
           ON TZD.[TimeZoneId] = TZ.[Id]
    WHERE  AAG.UserId = @adminUserId
           OR ISNULL(@adminUserId, 0) = 0
		   AND (AAG.[ID] = @Id
            OR ISNULL(@Id, 0) = 0)
		 AND ([DGM].DeviceId = @DeviceId
            OR ISNULL(@DeviceId, 0) = 0)
		AND( USG.Id = @UserGroupId
            OR ISNULL(@UserGroupId, 0) = 0)
		AND( [UGM].UserId = @UserId
            OR ISNULL(@UserId, 0) = 0)
        OR EXISTS (SELECT Id
                      FROM   [dbo].[USER]
                      WHERE  IsMasterAdmin = 1
                             AND ID = @AdminUserId);
END