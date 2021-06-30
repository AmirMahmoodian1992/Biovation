Create PROCEDURE [dbo].[InsertManualPlateDetectionLog]
@DetectorId INT, @UserId BIGINT, @ParentLogId BIGINT, @EventId INT, @LicensePlateId  INT , @LogDateTime DATETIME, @Ticks BIGINT, @DetectionPrecision TINYINT, @FullImage VARBINARY (MAX)=NULL, @PlateImage VARBINARY (MAX)=NULL, @InOrOut TINYINT = 0
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ثبت با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0, @Logid AS INT = 0;
    BEGIN TRY
        BEGIN TRANSACTION T1;
        BEGIN
            IF NOT EXISTS (SELECT ID
                           FROM   [PlateDetectionLog]
                           WHERE  [DetectorId] = @DetectorId
                                  AND [EventId] = @EventId
                                  AND [LicensePlateId] = @LicensePlateid
                                  AND ([LogDateTime] = @LogDateTime
                                       OR [Ticks] = @Ticks))
                BEGIN
                    INSERT  INTO [Rly].ManualPlateDetectionLog([DetectorId], [UserId], [ParentLogId], [EventId], [LicensePlateid], [LogDateTime], [Ticks], [DetectionPrecision], [SuccessTransfer], [TimeStamp], [InOrOut])
                    VALUES                                (@DetectorId, @UserId, @ParentLogId, @EventId, @LicensePlateId, @LogDateTime, @Ticks, @DetectionPrecision, 0, GETDATE(), @InOrOut);
                    SET @Logid = SCOPE_IDENTITY();
                    SET @Code = @Logid;
                    EXECUTE InsertPlateDetectionLogPicture @Logid, @FullImage, @PlateImage;
                END
        END
        COMMIT TRANSACTION T1;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1;
                SET @Validate = 0;
                SET @Message = N'ثبت انجام نشد';
            END
    END CATCH

    SELECT @Message AS [Message],
           @Code AS Id,
           @Code AS Code,
           @Validate AS Validate;
END