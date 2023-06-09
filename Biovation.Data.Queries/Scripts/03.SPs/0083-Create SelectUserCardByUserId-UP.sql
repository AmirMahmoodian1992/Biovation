
CREATE PROCEDURE [dbo].[SelectUserCardByUserId]
@UserId INT
AS
BEGIN
    SELECT [Id],
           [CardNum],
           [UserId],
           [IsActive],
           [IsDeleted]
    FROM   [dbo].[UserCard]
    WHERE  @UserId = [UserId]
           AND [IsDeleted] = 0;
END
GO
