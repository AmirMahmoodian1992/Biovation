
CREATE PROCEDURE [dbo].[ModifyUserCard]
@Id INT, @UserId INT, @CardNum NVARCHAR (50), @DataCheck INT=NULL
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'دخیره با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0;
    BEGIN TRY
        BEGIN TRANSACTION T1;
        BEGIN
            IF @Id = 0
                BEGIN
                    UPDATE [UserCard]
                    SET    IsActive   = 0,
                           ModifiedAt = GETDATE()
                    WHERE  CardNum = @CardNum
                           OR UserId = @UserId;
                    INSERT  INTO [UserCard] (UserId, CardNum, DataCheck, IsActive, IsDeleted, CreatedAt)
                    VALUES                 (@UserId, @CardNum, @DataCheck, 1, 0, GETDATE());
                END
        END
        SET @Code = SCOPE_IDENTITY();
        COMMIT TRANSACTION T1;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1;
                SET @Validate = 0;
                SET @Message = N'ذخیره انجام نشد';
            END
    END CATCH
    SELECT @Message AS [Message],
           @Code AS Id,
           @Validate AS Validate;
END
GO
