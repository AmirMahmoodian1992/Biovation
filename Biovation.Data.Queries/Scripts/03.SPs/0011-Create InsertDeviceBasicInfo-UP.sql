
CREATE PROCEDURE [dbo].[InsertDeviceBasicInfo]
@ID INT, @DeviceModelID INT, @Name NVARCHAR (100), @Active BIT=1, @IPAddress NVARCHAR (16), @Port INT, @MacAddress NVARCHAR (100)=NULL, @RegisterDate DATETIME, @HardwareVersion NVARCHAR (100)=NULL, @FirmwareVersion NVARCHAR (100)=NULL, @DeviceLockPassword NVARCHAR (16)=NULL, @SSL BIT=0, @TimeSync BIT, @SerialNumber NVARCHAR (100)=NULL, @DeviceTypeId INT,@Code bigint
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @LastCode AS INT = 0;
    BEGIN TRY
        BEGIN TRANSACTION T1;
        BEGIN
            IF NOT EXISTS (SELECT ID
                           FROM   Device
                           WHERE  ID = @ID)
                BEGIN
                    INSERT  INTO [dbo].[Device] ([ID], [DeviceModelID], [Name], [Active], [IPAddress], [Port], [MacAddress], [RegisterDate], [HardwareVersion], [FirmwareVersion], [DeviceLockPassword], [SSL], [TimeSync], [SerialNumber], [DeviceTypeId],[Code])
                    VALUES                     (@ID, @DeviceModelID, @Name, @Active, @IPAddress, @Port, @MacAddress, @RegisterDate, @HardwareVersion, @FirmwareVersion, @DeviceLockPassword, @SSL, @TimeSync, @SerialNumber, @DeviceTypeId,@Code);
                END
            ELSE
                BEGIN
                    SET @Message = N'شناسه دستگاه تکراری می باشد';
                    SET @Validate = 0;
                END
        END
        SET @LastCode = SCOPE_IDENTITY();
        COMMIT TRANSACTION T1;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1;
                SET @Validate = 0;
                SET @Message = N'ایجاد انجام نشد';
            END
    END CATCH
    SELECT @Message AS [Message],
           @ID AS Id,
           @Validate AS Validate;
END
GO
