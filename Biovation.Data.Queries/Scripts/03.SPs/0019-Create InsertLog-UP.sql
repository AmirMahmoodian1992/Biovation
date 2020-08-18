
CREATE PROCEDURE [dbo].[InsertLog]
@DeviceId INT,
@EventId INT,
@UserId INT,
@DateTime DATETIME,
@Ticks BIGINT,
@SubEvent INT,
@TNAEvent INT,
@InOutMode INT,
@MatchingType INT,
@SuccessTransfer bit,
@Image nvarchar(1000) = NULL
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ثبت با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0
    BEGIN TRY
        BEGIN TRANSACTION T1
        BEGIN
            IF NOT EXISTS (SELECT ID
                           FROM   [Log]
                           WHERE  [DeviceId] = @DeviceId
                                  AND [EventId] = @EventId
                                  AND [UserId] = @UserId
                                  AND ([DateTime] = @DateTime OR [Ticks] = @Ticks))
                BEGIN
                    INSERT  INTO [dbo].[Log] ([DeviceId], [EventId], [UserId], [DateTime], [Ticks], [SubEvent], [TNAEvent], [InOutMode], [MatchingType],[SuccessTransfer],[Image],[CreateAt])
                    VALUES                  (@DeviceId, @EventId, @UserId, @DateTime, @Ticks, @SubEvent, @TNAEvent, @InOutMode, @MatchingType, @SuccessTransfer, @Image,GETDATE())
                END
        END
        SET @Code = SCOPE_IDENTITY()
        COMMIT TRANSACTION T1
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1
                SET @Validate = 0
                SET @Message = N'ثبت انجام نشد'
            END
    END CATCH
    SELECT @Message AS [Message],
           @Code AS Id,
           @Validate AS Validate
END