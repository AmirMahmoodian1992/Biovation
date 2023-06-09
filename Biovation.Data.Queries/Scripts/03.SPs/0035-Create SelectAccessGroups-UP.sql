
CREATE PROCEDURE [dbo].[SelectAccessGroups]
@adminUserId INT=0, @DeviceId INT=0,@UserGroupId INT=0,@deviceGroupId INT=0, @UserId INT=0 , @pageNumber INT = NULL, @PageSize INT = Null
AS
BEGIN
DECLARE  @HasPaging  BIT;
	 SET @HasPaging = CASE
                                 WHEN @PageSize IS NOT NULL
                                      AND @PageNumber IS NOT NULL THEN
                                     1
                                 ELSE
                                     0
                             END;
 IF @HasPaging = 1
  BEGIN
    SELECT [AG].[ID] AS Data_ID,
           [AG].[Name] AS Data_Name,
           [AG].[TimeZoneId] AS Data_TimeZoneId,
           [USG].[Id] AS Data_UserGroup_Id ,
           [USG].[Name] AS Data_UserGroup_Name,
           [DG].[Id] AS Data_DeviceGroup_Id,
           [DG].[Name] AS Data_DeviceGroup_Name,
           [AG].[Description] AS Data_Description,
           [AAG].UserId AS Data_AdminUserId_Id,
           [UGM].[UserId] AS Data_UserGroup_Users_UserId,
           [UGM].[UserType] AS Data_UserGroup_Users_UserType,
           [UGM].[UserTypeTitle] AS Data_UserGroup_Users_UserTypeTitle,
           [DGM].[DeviceId] AS Data_DeviceGroup_Devices_DeviceId,
           [TZ].[Id] AS Data_TimeZone_Id,
           [TZ].[Name] AS Data_TimeZone_Name,
           [TZD].[Id] AS Data_TimeZone_Details_Id,
           [TZD].[TimeZoneId] AS Data_TimeZone_Details_TimeZoneId,
           [TZD].[DayNumber] AS Data_TimeZone_Details_DayNumber,
           [TZD].[FromTime] AS Data_TimeZone_Details_FromTime,
           [TZD].[ToTime] AS Data_TimeZone_Details_ToTime,
		   (@PageNumber-1)*@PageSize  AS [from],
		   @PageNumber AS PageNumber,
		   @PageSize As PageSize,
		   count(*) OVER() AS [Count]
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
              AND ([DGM].DeviceId = @DeviceId
                   OR ISNULL(@DeviceId, 0) = 0)
			AND (isnull(@DevicegroupId, 0) = 0
                OR @DeviceGroupId = [AGD].[DeviceGroupId])
              AND (USG.Id = @UserGroupId
                   OR ISNULL(@UserGroupId, 0) = 0)
              AND ([UGM].UserId = @UserId
                   OR ISNULL(@UserId, 0) = 0)
           OR EXISTS (SELECT Id
                      FROM   [dbo].[USER]
                      WHERE  IsMasterAdmin = 1
                             AND ID = @AdminUserId)
		   ORDER BY [AG].[ID]
           OFFSET (@PageNumber-1)*@PageSize ROWS
           FETCH NEXT @PageSize ROWS ONLY
END
Else
   BEGIN
    SELECT [AG].[ID] AS Data_ID,
           [AG].[Name] AS Data_Name,
           [AG].[TimeZoneId] AS Data_TimeZoneId,
           [USG].[Id] AS Data_UserGroup_Id ,
           [USG].[Name] AS Data_UserGroup_Name,
           [DG].[Id] AS Data_DeviceGroup_Id,
           [DG].[Name] AS Data_DeviceGroup_Name,
           [AG].[Description] AS Data_Description,
           [AAG].UserId AS Data_AdminUserId_Id,
           [UGM].[UserId] AS Data_UserGroup_Users_UserId,
           [UGM].[UserType] AS Data_UserGroup_Users_UserType,
           [UGM].[UserTypeTitle] AS Data_UserGroup_Users_UserTypeTitle,
           [DGM].[DeviceId] AS Data_DeviceGroup_Devices_DeviceId,
           [TZ].[Id] AS Data_TimeZone_Id,
           [TZ].[Name] AS Data_TimeZone_Name,
           [TZD].[Id] AS Data_TimeZone_Details_Id,
           [TZD].[TimeZoneId] AS Data_TimeZone_Details_TimeZoneId,
           [TZD].[DayNumber] AS Data_TimeZone_Details_DayNumber,
           [TZD].[FromTime] AS Data_TimeZone_Details_FromTime,
           [TZD].[ToTime] AS Data_TimeZone_Details_ToTime,
		   1  AS [from],
		   1 AS PageNumber,
		   count(*) OVER() As PageSize,
		   count(*) OVER() AS [Count]
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
              AND ([DGM].DeviceId = @DeviceId
                   OR ISNULL(@DeviceId, 0) = 0)
              AND (USG.Id = @UserGroupId
                   OR ISNULL(@UserGroupId, 0) = 0)
			  AND (isnull(@DevicegroupId, 0) = 0
                OR @DeviceGroupId = [AGD].[DeviceGroupId])
              AND ([UGM].UserId = @UserId
                   OR ISNULL(@UserId, 0) = 0)
           OR EXISTS (SELECT Id
                      FROM   [dbo].[USER]
                      WHERE  IsMasterAdmin = 1
                             AND Code = @AdminUserId);
   END
   END
GO
