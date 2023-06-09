CREATE PROCEDURE [dbo].[SelectUserGroups]
@Id INT=NULL,@adminUserId BIGINT=0,@accessGroupId INT=NULL,@userId INT=NULL,@PageNumber AS INT=0, @PageSize AS INT =0
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
    SELECT [UG].[Id] AS Data_Id,
           [UG].[Name] AS Data_Name,
           [UG].[AccessGroup] AS Data_AccessGroup,
           [UG].[Description] AS Data_Description,
           isnull(u.FirstName, '') + ' ' + isnull(u.SurName, '') AS Data_Users_UserName,
           [UGM].[Id] AS Data_Users_Id,
           [UGM].[UserId] AS Data_Users_UserId,
           [u].Code AS Users_UserCode,
           [UGM].[GroupId] AS Data_Users_GroupId,
           [UGM].[UserType] AS Data_Users_UserType,
           [UGM].[UserTypeTitle] AS Data_Users_UserTypeTitle,
		   (@PageNumber-1)*@PageSize  AS [from],
		   @PageNumber AS PageNumber,
		   @PageSize As PageSize,
		   count(*) OVER() AS [Count],
		   @Message AS e_Message,
		   @Code  AS e_Code,
		   @Validate  AS e_Validate
    FROM   [dbo].[UserGroup] AS UG
           LEFT OUTER JOIN
           [dbo].[UserGroupMember] AS UGM
           ON UGM.GroupId = UG.Id
           LEFT OUTER JOIN
           [dbo].[User] AS u
           ON ugm.UserId = u.ID
           LEFT OUTER JOIN
           [dbo].[AccessGroupUser] AS agu
           ON agu.UserGroupId = ug.Id
           LEFT OUTER JOIN
           [dbo].[AccessGroup] AS ag
           ON ag.Id = agu.AccessGroupId
           LEFT OUTER JOIN
           [dbo].[AdminAccessGroup] AS AAG
           ON AG.Id = AAG.AccessGroupId
    WHERE  (ISNULL(@adminUserId, 0) = 0
            OR U.Code = @AdminUserId
            OR EXISTS (SELECT Id
                       FROM   [dbo].[USER]
                       WHERE  IsMasterAdmin = 1
                              AND COde = @AdminUserId))
				AND ([UG].[Id] = @Id
                OR ISNULL(@Id, 0) = 0)			
				AND ([UGM].[UserId] = @UserId
                OR ISNULL(@UserId, 0) = 0)	
			    AND ([AGU].[AccessGroupId] = @accessGroupId
                OR ISNULL(@accessGroupId, 0) = 0)
		   ORDER BY [UG].[Id]
           OFFSET (@PageNumber-1)*@PageSize ROWS
           FETCH NEXT @PageSize ROWS ONLY	
END
ELSE
BEGIN
 SELECT [UG].[Id] AS Data_Id,
           [UG].[Name] AS Data_Name,
           [UG].[AccessGroup] AS Data_AccessGroup,
           [UG].[Description] AS Data_Description,
           isnull(u.FirstName, '') + ' ' + isnull(u.SurName, '') AS Data_Users_UserName,
           [UGM].[Id] AS Data_Users_Id,
           [UGM].[UserId] AS Data_Users_UserId,
           [u].Code AS Users_UserCode,
           [UGM].[GroupId] AS Data_Users_GroupId,
           [UGM].[UserType] AS Data_Users_UserType,
           [UGM].[UserTypeTitle] AS Data_Users_UserTypeTitle,
           1 AS [from],
		   1 AS PageNumber,
		   count(*) OVER() As PageSize,
		   count(*) OVER() AS [Count],
		   @Message AS e_Message,
		   @Code  AS e_Code,
		   @Validate  AS e_Validate
    FROM   [dbo].[UserGroup] AS UG
           LEFT OUTER JOIN
           [dbo].[UserGroupMember] AS UGM
           ON UGM.GroupId = UG.Id
           LEFT OUTER JOIN
           [dbo].[User] AS u
           ON ugm.UserId = u.ID
           LEFT OUTER JOIN
           [dbo].[AccessGroupUser] AS agu
           ON agu.UserGroupId = ug.Id
           LEFT OUTER JOIN
           [dbo].[AccessGroup] AS ag
           ON ag.Id = agu.AccessGroupId
           LEFT OUTER JOIN
           [dbo].[AdminAccessGroup] AS AAG
           ON AG.Id = AAG.AccessGroupId
    WHERE  (ISNULL(@adminUserId, 0) = 0
            OR U.Code = @AdminUserId
            OR EXISTS (SELECT Id
                       FROM   [dbo].[USER]
                       WHERE  IsMasterAdmin = 1
                              AND Code = @AdminUserId))
				AND ([UG].[Id] = @Id
                OR ISNULL(@Id, 0) = 0)			
				AND ([UGM].[UserId] = @UserId
                OR ISNULL(@UserId, 0) = 0)	
			    AND ([AGU].[AccessGroupId] = @accessGroupId
                OR ISNULL(@accessGroupId, 0) = 0);
END
END
