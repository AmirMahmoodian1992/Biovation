
CREATE PROCEDURE [dbo].[UpdateDeviceBasicInfoById]
@Id BIGINT, @DeviceModelId INT, @Name NVARCHAR (100), @Active BIT=1, @IPAddress NVARCHAR (16), @Port INT, @MacAddress NVARCHAR (100)=NULL, @RegisterDate DATETIME, @HardwareVersion NVARCHAR (100)=NULL, @FirmwareVersion NVARCHAR (100)=NULL, @DeviceLockPassword NVARCHAR (16)=NULL, @SSL BIT=0, @TimeSync BIT, @SerialNumber NVARCHAR (100)=NULL, @DeviceTypeId INT
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0;
    BEGIN TRY
        BEGIN TRANSACTION T1;
        BEGIN
            IF EXISTS (SELECT Id
                       FROM   Device
                       WHERE  Id = @Id)
                BEGIN
                    UPDATE [dbo].[Device]
                    SET    [DeviceModelId]      = @DeviceModelId,
                           [Name]               = @Name,
                           [Active]             = @Active,
                           [IPAddress]          = @IPAddress,
                           [Port]               = @Port,
                           [MacAddress]         = @MacAddress,
                           [RegisterDate]       = @RegisterDate,
                           [HardwareVersion]    = @HardwareVersion,
                           [FirmwareVersion]    = @FirmwareVersion,
                           [DeviceLockPassword] = @DeviceLockPassword,
                           [SSL]                = @SSL,
                           [TimeSync]           = @TimeSync,
                           [SerialNumber]       = @SerialNumber,
                           [DeviceTypeId]       = @DeviceTypeId
                    WHERE  Id = @Id;
                END
        END
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
           @Id AS Id,
           @Validate AS Validate;
END
GO
