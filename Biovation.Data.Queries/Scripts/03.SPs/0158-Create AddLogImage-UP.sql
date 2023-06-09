
CREATE PROCEDURE [dbo].[AddLogImage]
@DateTime datetime,
@UserId int,
@DeviceId int,
@EventId int,
@ImageFilePath nvarchar(MAX)
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ویرایش با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0
    BEGIN TRY
        BEGIN TRANSACTION T1
        BEGIN
            UPDATE [dbo].[Log]
            SET    [Image] = @ImageFilePath
            WHERE  [DateTime] = @DateTime AND [UserId] = @UserId AND [DeviceId] = @DeviceId AND [EventId] = @EventId
        END
        SET @Code = SCOPE_IDENTITY()
        COMMIT TRANSACTION T1
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1
                SET @Validate = 0
                SET @Message = N'ویرایش انجام نشد'
            END
    END CATCH
    SELECT @Message AS [Message],
           @Code AS Id,
           @Validate AS Validate
END
GO
