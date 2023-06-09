CREATE PROCEDURE [dbo].[SelectAdminDevicesByUserIdByPaging]
@UserId BIGINT,@PageNumber AS INT=0,@PageSize AS INT =0
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
    SELECT DISTINCT [AD].[Id] AS Data_Id,
                    [AD].[UserId] as Data_UserId,
                    CASE WHEN AD.[DeviceId] IS NOT NULL THEN AD.[DeviceId] WHEN [DeviceGroupId] IS NOT NULL THEN DGM.DeviceId END AS Data_DeviceId,
           (@PageNumber-1)*@PageSize  AS [from],
		   @PageNumber AS PageNumber,
		   @PageSize As PageSize,
		   count(*) OVER() AS [Count],
           @Message AS e_Message,
		   @Code  AS e_Code,
		   @Validate  AS e_Validate
	FROM   [dbo].[AdminDevice] AS AD
           LEFT OUTER JOIN
           [dbo].[DeviceGroupMember] AS DGM
           ON AD.DeviceGroupId = DGM.GroupId
    WHERE  AD.UserId = @UserId
	ORDER BY AD.UserId
           OFFSET (@PageNumber-1)*@PageSize ROWS
           FETCH NEXT @PageSize ROWS ONLY
END
ELSE
BEGIN
  SELECT DISTINCT [AD].[Id] AS Data_Id,
           [AD].[UserId] as Data_UserId,
           CASE WHEN AD.[DeviceId] IS NOT NULL THEN AD.[DeviceId] WHEN [DeviceGroupId] IS NOT NULL THEN DGM.DeviceId END AS Data_DeviceId,
           1  AS [from],
		   1 AS PageNumber,
		   count(*) OVER() As PageSize,
		   count(*) OVER() AS [Count],
           @Message AS e_Message,
		   @Code  AS e_Code,
		   @Validate  AS e_Validate
	FROM   [dbo].[AdminDevice] AS AD
           LEFT OUTER JOIN
           [dbo].[DeviceGroupMember] AS DGM
           ON AD.DeviceGroupId = DGM.GroupId
    WHERE  AD.UserId = @UserId;
END
END