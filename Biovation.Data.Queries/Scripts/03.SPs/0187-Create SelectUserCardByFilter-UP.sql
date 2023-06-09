CREATE PROCEDURE [dbo].[SelectUserCardByFilter]
@UserId INT,@IsActive BIT=0,@PageNumber AS INT=NULL, @PageSize AS INT=NULL
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
    SELECT [Id] AS Data_Id,
           [CardNum] AS Data_CardNum,
           [UserId] AS Data_UserId ,
           [IsActive]  AS Data_IsActive,
           [IsDeleted]  AS Data_IsDeleted,
		   (@PageNumber-1)*@PageSize  AS [from],
		   @PageNumber AS PageNumber,
		   @PageSize As PageSize,
		   count(*) OVER() AS [Count]
    FROM   [dbo].[UserCard] UC
    WHERE (UC.IsActive = @IsActive
           OR ISNULL(@IsActive, 0) = 0)
		   AND (UC.UserId = @UserId
            OR ISNULL(@UserId, 0) = 0)
	       ORDER BY UC.Id
           OFFSET (@PageNumber-1)*@PageSize ROWS
           FETCH NEXT @PageSize ROWS ONLY
END
Else
BEGIN
           SELECT [Id] AS Data_Id,
           [CardNum] AS Data_CardNum,
           [UserId] AS Data_UserId ,
           [IsActive]  AS Data_IsActive,
           [IsDeleted]  AS Data_IsDeleted,
           1  AS [from],
		   1 AS PageNumber,
		   count(*) OVER() As PageSize,
		   count(*) OVER() AS [Count]
    FROM   [dbo].[UserCard] UC
    WHERE (UC.IsActive = @IsActive
           OR ISNULL(@IsActive, 0) = 0)
		   AND (UC.UserId = @UserId
            OR ISNULL(@UserId, 0) = 0)
END
END