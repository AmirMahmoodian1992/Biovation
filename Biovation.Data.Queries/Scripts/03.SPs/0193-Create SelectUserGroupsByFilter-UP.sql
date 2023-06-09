Create PROCEDURE [dbo].[SelectUserGroupsByFilter]
@Id INT=null,@adminUserId BIGINT=0,@UserId INT=null,@accessGroupId INT=null,@PageNumber AS INT=0, @PageSize AS INT=0
AS
DECLARE  @HasPaging   BIT;
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
    SELECT [UG].[Id] As Data_Id,
           [UG].[Name] As Data_Name,
           [UG].[AccessGroup] As Data_AccessGroup,
           [UG].[Description] As Data_Description,
           isnull(u.FirstName, '') + ' ' + isnull(u.SurName, '') AS Data_Users_UserName,
           [UGM].[Id] AS Data_Users_Id,
           [UGM].[UserId] AS Data_Users_UserId,
           [UGM].[GroupId] AS Data_Users_GroupId,
           [UGM].[UserType] AS Data_Users_UserType,
           [UGM].[UserTypeTitle] AS Data_Users_UserTypeTitle,
		   (@PageNumber-1)*@PageSize  AS [from],
		   @PageNumber AS PageNumber,
		   @PageSize As PageSize,
		   count(1) OVER() AS [Count]
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
            OR AAG.UserId = @AdminUserId
            OR EXISTS (SELECT Id
                       FROM   [dbo].[USER]
                       WHERE  IsMasterAdmin = 1
                              AND ID = @AdminUserId))
			AND [UGM].[UserId] = @UserId
            OR ISNULL(@UserId, 0) = 0
			AND [AGU].[AccessGroupId] = @accessGroupId
            OR ISNULL(@accessGroupId, 0) = 0
			AND [UG].[Id] = @Id
			OR ISNULL(@Id, 0) = 0
		   ORDER BY [UG].[Id]
           OFFSET (@PageNumber-1)*@PageSize ROWS
           FETCH NEXT @PageSize ROWS ONLY
END
ElSE
BEGIN
    SELECT [UG].[Id] As Data_Id,
           [UG].[Name] As Data_Name,
           [UG].[AccessGroup] As Data_AccessGroup,
           [UG].[Description] As Data_Description,
           isnull(u.FirstName, '') + ' ' + isnull(u.SurName, '') AS Data_Users_UserName,
           [UGM].[Id] AS Data_Users_Id,
           [UGM].[UserId] AS Data_Users_UserId,
           [UGM].[GroupId] AS Data_Users_GroupId,
           [UGM].[UserType] AS Data_Users_UserType,
           [UGM].[UserTypeTitle] AS Data_Users_UserTypeTitle,
           1  AS [from],
		   1 AS PageNumber,
		   count(1) OVER() As PageSize,
		   count(1) OVER() AS [Count]
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
            OR AAG.UserId = @AdminUserId
            OR EXISTS (SELECT Id
                       FROM   [dbo].[USER]
                       WHERE  IsMasterAdmin = 1
                              AND ID = @AdminUserId))
			AND [UGM].[UserId] = @UserId
            OR ISNULL(@UserId, 0) = 0
			AND [AGU].[AccessGroupId] = @accessGroupId
            OR ISNULL(@accessGroupId, 0) = 0
			AND [UG].[Id] = @Id
			OR ISNULL(@Id, 0) = 0
END
END