CREATE PROCEDURE [dbo].[SelectAdminDeviceGroupsByUserIdByPaging]
@UserId BIGINT,@PageNumber AS INT=NULL, @PageSize AS INT=NULL
AS
DECLARE  @HasPaging   BIT;
DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (15) = N'200';
   
BEGIN
 SET @HasPaging = CASE
                                WHEN @PageSize =0
                                      AND @PageNumber =0 THEN
                                     0                                   
                                 ELSE
                                     1
                             END;
 IF @HasPaging = 1
     BEGIN
    SELECT ad.Id as Data_id,
           ad.UserId as Data_UserId,
           CASE WHEN DeviceId IS NOT NULL THEN d.ID WHEN DeviceGroupId IS NOT NULL THEN CAST (dg.Id AS BIGINT) END AS Data_GroupDeviceId,
           CASE WHEN DeviceId IS NOT NULL THEN 0 WHEN DeviceGroupId IS NOT NULL THEN 1 END AS Data_TypeId,
           CASE WHEN DeviceId IS NOT NULL THEN N'دستگاه' WHEN DeviceGroupId IS NOT NULL THEN N'گروه دستگاه' END AS Data_type,
           CASE WHEN DeviceId IS NOT NULL THEN d.[Name] WHEN DeviceGroupId IS NOT NULL THEN dg.[Name] END AS Data_Name,
		   (@PageNumber-1)*@PageSize  AS [from],
		   @PageNumber AS PageNumber,
		   @PageSize As PageSize,
		   count(*) OVER() AS [Count],
		   @Message AS e_Message,
		   @Code  AS e_Code,
		   @Validate  AS e_Validate   
	FROM   [dbo].AdminDevice AS ad
           LEFT OUTER JOIN
           dbo.Device AS d
           ON (d.ID = ad.DeviceId
               AND ad.DeviceId IS NOT NULL)
           LEFT OUTER JOIN
           dbo.DeviceGroup AS dg
           ON (dg.Id = ad.DeviceGroupId
               AND ad.DeviceGroupId IS NOT NULL)
    WHERE  ad.UserId = @UserId
	ORDER BY ad.Id
           OFFSET (@PageNumber-1)*@PageSize ROWS
           FETCH NEXT @PageSize ROWS ONLY
END
ELSE
BEGIN
    SELECT ad.Id as Data_id,
           ad.UserId as Data_UserId,
           CASE WHEN DeviceId IS NOT NULL THEN d.ID WHEN DeviceGroupId IS NOT NULL THEN CAST (dg.Id AS BIGINT) END AS Data_GroupDeviceId,
           CASE WHEN DeviceId IS NOT NULL THEN 0 WHEN DeviceGroupId IS NOT NULL THEN 1 END AS Data_TypeId,
           CASE WHEN DeviceId IS NOT NULL THEN N'دستگاه' WHEN DeviceGroupId IS NOT NULL THEN N'گروه دستگاه' END AS Data_type,
           CASE WHEN DeviceId IS NOT NULL THEN d.[Name] WHEN DeviceGroupId IS NOT NULL THEN dg.[Name] END AS Data_Name,
           1 AS [from],
		   1 AS PageNumber,
		   count(*) OVER() As PageSize,
		   count(*) OVER() AS [Count],
		   @Message AS e_Message,
		   @Code  AS e_Code,
		   @Validate  AS e_Validate   
	FROM   [dbo].AdminDevice AS ad
           LEFT OUTER JOIN
           dbo.Device AS d
           ON (d.ID = ad.DeviceId
               AND ad.DeviceId IS NOT NULL)
           LEFT OUTER JOIN
           dbo.DeviceGroup AS dg
           ON (dg.Id = ad.DeviceGroupId
               AND ad.DeviceGroupId IS NOT NULL)
    WHERE  ad.UserId = @UserId
END
END