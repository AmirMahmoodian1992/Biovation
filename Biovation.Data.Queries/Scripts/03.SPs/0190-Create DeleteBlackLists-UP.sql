CREATE PROCEDURE [dbo].[DeleteBlackLists]
@json VARCHAR(100)
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'حذف با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (15) = N'200';
    BEGIN TRY
        BEGIN TRANSACTION T1;

DELETE FROM [dbo].[BlackList]
WHERE Id IN 
(
SELECT  value
FROM OPENJSON(@json)
)
        COMMIT TRANSACTION T1;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1;
                SET @Validate = 0;
                SET @Message = N'حذف انجام نشد';
                SET @Code =400;
            END
    END CATCH
    SELECT @Message AS [Message],
           @Validate AS Validate,
           @Code AS Code
END
GO