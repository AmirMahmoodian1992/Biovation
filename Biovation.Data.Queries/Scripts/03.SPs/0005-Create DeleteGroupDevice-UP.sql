
CREATE PROCEDURE [dbo].[DeleteGroupDevice]
@GroupId INT
AS
BEGIN
    BEGIN TRY
        DECLARE @Message AS NVARCHAR (200) = N'حذف با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0;
        BEGIN TRANSACTION T1;
        DELETE [dbo].[DeviceGroupMember]
        WHERE  GroupId = @GroupId;
        DELETE [dbo].[DeviceGroup]
        WHERE  Id = @GroupId;
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
           @GroupId AS Id,
           @Validate AS Validate;
END
GO
