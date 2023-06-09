
CREATE PROCEDURE [dbo].[DeleteUserCard]
@Id INT
AS
BEGIN
    BEGIN TRY
        DECLARE @Message AS NVARCHAR (200) = N'حذف با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (15) = N'201';
        BEGIN TRANSACTION T1;
        UPDATE [dbo].[UserCard]
        SET    IsDeleted  = 1,
               IsActive   = 0,
               ModifiedAt = GETDATE()
        WHERE  Id = @Id;
        COMMIT TRANSACTION T1;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1;
                SET @Validate = 0;
                SET @Code = 400;
                SET @Message = N'حذف انجام نشد';
            END
    END CATCH
    SELECT @Message AS [Message],
           @Id AS Id,
           @Code AS Code,
           @Validate AS Validate;
END
GO
