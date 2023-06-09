Create PROCEDURE [dbo].[InsertPlateDetectionLogPicture]
@Id INT, @FullImage VARBINARY (MAX)=NULL, @PlateImage VARBINARY (MAX)=NULL
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ثبت با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0, @LicensePlateid AS INT = 0;
    BEGIN TRY
        BEGIN TRANSACTION T1;
        BEGIN
            INSERT  INTO [dbo].[PlateDetectionPictureLog] ([LogId], [FullImage], [PlateImage])
            VALUES                                       (@id, @FullImage, @PlateImage);
            SET @Code = SCOPE_IDENTITY();
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
END