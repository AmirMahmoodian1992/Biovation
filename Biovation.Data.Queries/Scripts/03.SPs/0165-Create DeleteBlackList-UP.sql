CREATE PROCEDURE [dbo].[DeleteBlackList]
@Id INT
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'حذف  با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (15) = N'200'
    BEGIN TRY
        BEGIN TRANSACTION T1;
        IF NOT EXISTS (SELECT ID
                       FROM   BlackList
                       WHERE  ID = @Id)
            BEGIN
                SET @Message = N'شناسه لیست سیاه موجود  نمی باشد';
                SET @Validate = 0;
            END
        ELSE
            BEGIN
                BEGIN
                    UPDATE [dbo].[BlackList]
                    SET    [IsDeleted] = 1
                    WHERE  ID = @Id;
                END
            END
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
           @Validate AS Validate,
           @Code AS Code
           
END