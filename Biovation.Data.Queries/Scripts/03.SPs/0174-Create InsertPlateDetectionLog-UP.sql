Create PROCEDURE [dbo].[InsertPlateDetectionLog]
@DetectorId INT, @EventId INT, @LicensePlateid  INT , @LogDateTime DATETIME, @Ticks BIGINT, @DetectionPrecision TINYINT, @FullImage VARBINARY (MAX)=NULL, @PlateImage VARBINARY (MAX)=NULL, @InOrOut TINYINT = 0
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ثبت با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0, @Logid AS INT = 0;
    BEGIN TRY
        BEGIN TRANSACTION T1;
        BEGIN
            SELECT TOP 1 @Logid = ISNULL(ID, 0)
                           FROM   [PlateDetectionLog]
                           WHERE  [DetectorId] = @DetectorId
                                  AND [EventId] = @EventId
                                  AND [LicensePlateId] = @LicensePlateid
                                  AND ([LogDateTime] = @LogDateTime
                                       /*OR [Ticks] = @Ticks*/)
            IF ISNULL(@Logid, 0) = 0
                BEGIN
                    INSERT  INTO [dbo].[PlateDetectionLog] ([DetectorId], [EventId], [LicensePlateid], [LogDateTime], [Ticks], [DetectionPrecision], [SuccessTransfer], [TimeStamp], [InOrOut])
                    VALUES                                (@DetectorId, @EventId, @LicensePlateId, @LogDateTime, @Ticks, @DetectionPrecision, 0, GETDATE(),@InOrOut);
                    SET @Logid = SCOPE_IDENTITY();
                    SET @Code = @Logid;
                    IF @FullImage IS NOT NULL AND @PlateImage IS NOT NULL
                        EXECUTE InsertPlateDetectionLogPicture @Logid, @FullImage, @PlateImage;
                END
            ELSE
                SET @Code = 409 --Duplicate Error Code
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
           @Logid AS Id,
           @Code AS Code,
           @Validate AS Validate;
END