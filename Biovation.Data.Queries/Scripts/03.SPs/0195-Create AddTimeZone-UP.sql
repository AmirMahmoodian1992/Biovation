
CREATE PROCEDURE [dbo].[ATimeZone]
@Id INT, @Name NVARCHAR(100)
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @Code  AS nvarchar(5) = N'201';
    BEGIN TRY
        BEGIN TRANSACTION T1;
        BEGIN
            IF NOT EXISTS (SELECT ID
                           FROM   [dbo].[TimeZone]
                           WHERE  ID = @Id)
                BEGIN
                    INSERT INTO [dbo].[TimeZone]
									   ([Name])
									VALUES                  
									 (@Name);
                END

				SET @Message = N'ویرایش با موفقیت انجام شد';
                SET @Code  = 400;

        END
        SET @Code = SCOPE_IDENTITY();
        COMMIT TRANSACTION T1;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1;
                SET @Validate = 0;
                SET @Message = N'ایجاد انجام نشد';
            END
    END CATCH
    SELECT @Message AS [Message],
           @Code AS Id,
           @Code AS Code,
           @Validate AS Validate;
END
GO