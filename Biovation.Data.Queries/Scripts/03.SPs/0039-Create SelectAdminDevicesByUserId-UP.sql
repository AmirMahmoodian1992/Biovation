
CREATE PROCEDURE [dbo].[SelectAdminDevicesByUserId]
@UserId BIGINT,@pageNumber INT = 0, @PageSize INT = 0
AS
BEGIN
DECLARE  @HasPaging  BIT;
DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @Code_Result AS NVARCHAR (15) = N'200';
    SET @HasPaging = CASE WHEN @PageSize = 0
                               AND @PageNumber = 0 THEN 0 ELSE 1 END;
 IF @HasPaging = 1
BEGIN
    SELECT DISTINCT [UserId],
                    CASE WHEN AD.[DeviceId] IS NOT NULL THEN AD.[DeviceId] WHEN [DeviceGroupId] IS NOT NULL THEN DGM.DeviceId END AS DeviceId
           ,(@PageNumber-1)*@PageSize  AS [from],
		   @PageNumber AS PageNumber,
		   @PageSize As PageSize,
		   count(*) OVER() AS [Count],
		     @Message AS e_Message,
              @Code_Result AS e_Code,
              @Validate AS e_Validate
   FROM   [dbo].[AdminDevice] AS AD
           LEFT OUTER JOIN
           [dbo].[DeviceGroupMember] AS DGM
           ON AD.DeviceGroupId = DGM.GroupId
    WHERE  AD.UserId = @UserId
		  ORDER BY AD.UserId
            OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
END
ELSE
BEGIN
    SELECT DISTINCT [UserId],
                    CASE WHEN AD.[DeviceId] IS NOT NULL THEN AD.[DeviceId] WHEN [DeviceGroupId] IS NOT NULL THEN DGM.DeviceId END AS DeviceId
       		       , 1 AS [from],
                   1 AS PageNumber,
                   count(*) OVER () AS PageSize,
                   count(*) OVER () AS [Count],
                   @Message AS e_Message,
                   @Code_Result AS e_Code,
                   @Validate AS e_Validate
	FROM   [dbo].[AdminDevice] AS AD
           LEFT OUTER JOIN
           [dbo].[DeviceGroupMember] AS DGM
           ON AD.DeviceGroupId = DGM.GroupId
    WHERE  AD.UserId = @UserId;
END
END
GO