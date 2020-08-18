
CREATE PROCEDURE [dbo].[DeleteTimeZoneById]
@Id INT
AS
BEGIN
    BEGIN TRY
        DECLARE @Message AS NVARCHAR (200) = N'حذف با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0;
        BEGIN TRANSACTION T1;

		DELETE FROM [dbo].[TimeZoneDetail]
		WHERE  TimeZoneId = @Id;

       DELETE FROM [dbo].[TimeZone]
        WHERE  Id = @Id;

        COMMIT TRANSACTION T1;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1;
                SET @Validate = 0;
                SET @Message = N'حذف انجام نشد';
            END
    END CATCH
    SELECT @Message AS [Message],
           @Id AS Id,
           @Validate AS Validate;
END
GO
