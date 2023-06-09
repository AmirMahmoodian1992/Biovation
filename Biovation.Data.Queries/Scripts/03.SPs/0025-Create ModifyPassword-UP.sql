
CREATE PROCEDURE [dbo].[ModifyPassword]
@Id INT, @Password NVARCHAR (MAX)=NULL
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'دخیره با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (15) = N'201';
    BEGIN TRY
        BEGIN TRANSACTION T1;
        BEGIN
            UPDATE dbo.[User]
            SET    [Password] = @Password
            WHERE  [ID] = @Id
                   AND @Id != 0;
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
                 SET @Code = 400;
            END
    END CATCH
    SELECT TOP (1) @Message AS [Message],
                   @Validate AS [Validate],
                   @Code AS Code,
                   @Code AS Id;
END
GO
