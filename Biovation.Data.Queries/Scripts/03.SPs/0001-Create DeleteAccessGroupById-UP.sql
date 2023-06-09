
CREATE PROCEDURE [dbo].[DeleteAccessGroupById]
@Id INT
AS
BEGIN
    BEGIN TRY
        DECLARE @Message AS NVARCHAR (200) = N'حذف با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (15) = N'200'
        BEGIN TRANSACTION T1
        IF EXISTS (SELECT *
                   FROM   dbo.AccessGroup
                   WHERE  id = @id
                          AND IsDefault = 0)
            BEGIN
                DELETE [dbo].[AccessGroupDevice]
                WHERE  AccessGroupId = @Id
                DELETE [dbo].[AccessGroupUser]
                WHERE  AccessGroupId = @Id
                DELETE [dbo].[AccessGroup]
                WHERE  Id = @Id
				DELETE [dbo].[AdminAccessGroup]
				WHERE AccessGroupId = @Id
            END
        ELSE
            SET @Validate = 0
        SET @Message = N'گروه دسترسی پیش فرض را نمیتوان حذف کرد';
        SET  @Code = 400;
        COMMIT TRANSACTION T1
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1
                SET @Validate = 0
                SET @Message = N'حذف انجام نشد'
                SET  @Code = 400;
            END
    END CATCH
    SELECT @Message AS [Message],
           @Id AS Id,
           @Code AS Code,
           @Validate AS Validate
END
GO
