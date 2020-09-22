CREATE PROCEDURE [dbo].[ModifyAccessGroupAdminUser]
@Xml NVARCHAR (MAX), @AccessGroupId INT
AS
DECLARE @DocId AS INT, @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام شد', @Validate AS INT = 1
BEGIN
    SET LOCK_TIMEOUT 1800
    BEGIN TRY
        BEGIN TRANSACTION T1
        DECLARE @Id AS INT
        EXECUTE sp_xml_preparedocument @DocId OUTPUT, @Xml
        BEGIN
            DELETE [dbo].[AdminAccessGroup]
            WHERE  AccessGroupId = @AccessGroupId
            INSERT INTO [dbo].[AdminAccessGroup] (UserId, AccessGroupId)
            SELECT Id,
                   @AccessGroupId
            FROM   OPENXML (@DocId, '/Root/AdminUsers', 2) WITH (Id BIGINT)
        END
        EXECUTE sp_xml_removedocument @DocId
        COMMIT TRANSACTION T1
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1
                SET @Validate = 0
                SET @Message = N'خطا در بانک اطلاعاتی'
            END
    END CATCH
END
SELECT @Message AS [Message],
       @Validate AS Validate
