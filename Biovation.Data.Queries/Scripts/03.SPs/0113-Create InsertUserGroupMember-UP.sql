CREATE PROCEDURE [dbo].[InsertUserGroupMember]
@UserId BIGINT, @GroupId INT,@UserType INt = 1
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (15) = N'201';
    BEGIN TRY
        BEGIN TRANSACTION T1;
        
            --SET @Code = @Id;
            IF NOT EXISTS (SELECT *
                           FROM   [dbo].[UserGroupMember]
                           WHERE  UserId = @UserId and @GroupId = GroupId)
                BEGIN
                        INSERT  INTO [dbo].[UserGroupMember] ([UserId], [GroupId] , [UserType])
                        VALUES                          (@UserId, @GroupId,@UserType);
                END
           
            SET @Code = SCOPE_IDENTITY();
        COMMIT TRANSACTION T1;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1;
                SET @Validate = 0;
                SET @Code = 400;
                SET @Message = N'ایجاد انجام نشد';
            END
    END CATCH
    SELECT @Message AS [Message],
           @Code AS Id,
           @Code AS Code,
           @Validate AS Validate;
END