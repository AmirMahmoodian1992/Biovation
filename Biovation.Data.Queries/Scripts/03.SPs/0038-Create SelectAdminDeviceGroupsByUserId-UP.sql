
CREATE PROCEDURE [dbo].[SelectAdminDeviceGroupsByUserId]
@UserId BIGINT
AS
BEGIN
    SELECT ad.Id,
           ad.UserId,
           CASE WHEN DeviceId IS NOT NULL THEN d.ID WHEN DeviceGroupId IS NOT NULL THEN CAST (dg.Id AS BIGINT) END AS GroupDeviceId,
           CASE WHEN DeviceId IS NOT NULL THEN 0 WHEN DeviceGroupId IS NOT NULL THEN 1 END AS TypeId,
           CASE WHEN DeviceId IS NOT NULL THEN N'دستگاه' WHEN DeviceGroupId IS NOT NULL THEN N'گروه دستگاه' END AS [type],
           CASE WHEN DeviceId IS NOT NULL THEN d.[Name] WHEN DeviceGroupId IS NOT NULL THEN dg.[Name] END AS [Name]
    FROM   [dbo].AdminDevice AS ad
           LEFT OUTER JOIN
           dbo.Device AS d
           ON (d.ID = ad.DeviceId
               AND ad.DeviceId IS NOT NULL)
           LEFT OUTER JOIN
           dbo.DeviceGroup AS dg
           ON (dg.Id = ad.DeviceGroupId
               AND ad.DeviceGroupId IS NOT NULL)
    WHERE  ad.UserId = @UserId;
END
GO
