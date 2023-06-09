
CREATE PROCEDURE [dbo].[SelectActiveUserCardByUserId]
@UserId INT
AS
BEGIN
    SELECT   [Id],
             [CardNum],
             [UserId],
             [DataCheck],
             [IsActive],
             [IsDeleted]
    FROM     [dbo].[UserCard]
    WHERE    @UserId = UserId
             AND IsActive = 1
    ORDER BY CreatedAt DESC;
END
GO
