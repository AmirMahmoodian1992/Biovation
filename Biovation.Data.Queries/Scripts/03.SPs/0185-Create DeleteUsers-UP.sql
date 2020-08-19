CREATE PROCEDURE [dbo].[DeleteUsers]
@json VARCHAR(100)
AS
BEGIN
    BEGIN TRY
        DECLARE @Message AS NVARCHAR (200) = N'حذف با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0;
        BEGIN TRANSACTION T1;
        DELETE [dbo].[FingerTemplate]
       WHERE  UserId IN 
    (
    SELECT  value
    FROM OPENJSON(@json)
     )

		DELETE [dbo].[UserCard]
        WHERE  UserId IN 
    (
    SELECT  value
    FROM OPENJSON(@json)
     )

		DELETE [dbo].[FaceTemplate]
        WHERE  UserId IN 
    (
    SELECT  value
    FROM OPENJSON(@json)
     )

		DELETE [dbo].[UserGroupMember]
        WHERE  UserId IN 
    (
    SELECT  value
    FROM OPENJSON(@json)
     )

        DELETE [dbo].[User]
        WHERE  ID IN 
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
                SET @Message = N'حذف یوزر با مشکل مواجه شد';
            END
    END CATCH
    SELECT @Message AS [Message],
           --@Id AS Id,
           @Validate AS Validate;
END
GO