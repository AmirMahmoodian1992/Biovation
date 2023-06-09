CREATE PROCEDURE [dbo].[SelectAccessControlDeviceGroup]
@AccessControlId int,@PageNumber AS INT=0, @PageSize AS INT =0
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
    SELECT 
	       ug.Id AS Data_Id, ug.[Name] AS Data_Name , a.Id as 'AccessGroupId',
		   (@PageNumber-1)*@PageSize  AS [from],
		   @PageNumber AS PageNumber,
		   @PageSize As PageSize,
		   count(*) OVER() AS [Count],
		   @Message AS e_Message,
		   @Code  AS e_Code,
		   @Validate  AS e_Validate
    FROM   [dbo].[DeviceGroup] ug 
	inner join [dbo].AccessGroupDevice ag on ag.DeviceGroupId = ug.Id
	inner join [dbo].[AccessGroup] a on a.ID = ag.AccessGroupId
	where a.ID = @AccessControlId
	ORDER BY ug.Id
           OFFSET (@PageNumber-1)*@PageSize ROWS
           FETCH NEXT @PageSize ROWS ONLY	
END
ELSE
BEGIN
    SELECT 
	       ug.Id AS Data_Id, ug.[Name] AS Data_Name , a.Id as 'AccessGroupId',
		   1 AS [from],
		   1 AS PageNumber,
		   count(*) OVER() As PageSize,
		   count(*) OVER() AS [Count],
		   @Message AS e_Message,
		   @Code  AS e_Code,
		   @Validate  AS e_Validate
    FROM   [dbo].[DeviceGroup] ug 
	inner join [dbo].AccessGroupDevice ag on ag.DeviceGroupId = ug.Id
	inner join [dbo].[AccessGroup] a on a.ID = ag.AccessGroupId
	where a.ID = @AccessControlId

END
END
