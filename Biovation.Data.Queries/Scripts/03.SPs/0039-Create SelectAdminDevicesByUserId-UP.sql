
CREATE PROCEDURE [dbo].[SelectAdminDevicesByUserId]
@UserId BIGINT
AS
BEGIN
    SELECT DISTINCT [UserId],
                    CASE WHEN AD.[DeviceId] IS NOT NULL THEN AD.[DeviceId] WHEN [DeviceGroupId] IS NOT NULL THEN DGM.DeviceId END AS DeviceId
    FROM   [dbo].[AdminDevice] AS AD
           LEFT OUTER JOIN
           [dbo].[DeviceGroupMember] AS DGM
           ON AD.DeviceGroupId = DGM.GroupId
    WHERE  AD.UserId = @UserId;
END
GO
