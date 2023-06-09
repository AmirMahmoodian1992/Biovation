CREATE PROCEDURE [dbo].[ModifyDeviceBasicInfo]
@Id INT, 
@Code INT, 
@DeviceModelId INT, @Name NVARCHAR (100), @Active BIT=1, @IPAddress NVARCHAR (200), @Port INT, @MacAddress NVARCHAR (100)=NULL, @HardwareVersion NVARCHAR (100)=NULL, @FirmwareVersion NVARCHAR (100)=NULL, @DeviceLockPassword NVARCHAR (16)=NULL, @SSL BIT=0, @TimeSync BIT, @SerialNumber NVARCHAR (100)=NULL, @DeviceTypeId INT
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد/ویرایش با موفقیت انجام گرفت', @Validate AS INT = 1, @CodeINSERT  AS NVARCHAR (15) = N'201';
    BEGIN TRY
        BEGIN TRANSACTION T1;
        DECLARE @BRANDID AS INT;
        
        SELECT TOP 1 @BRANDID = B.code
        FROM   [dbo].[lookup] AS B
               INNER JOIN
               [dbo].DeviceModel AS M
               ON B.code = M.BrandId
        WHERE  M.Id = @DeviceModelId;

        --SELECT @Id = Id
        --FROM   [dbo].[Device]
        --WHERE  Code = @Code
        --       AND DeviceModelId = @deviceModelId;

        IF ISNULL(@Id, 0) = 0
            BEGIN
            IF EXISTS (SELECT TOP(1) 1
                           FROM   [dbo].Device
                           WHERE  Code = @Code
                                  AND DeviceModelId IN (SELECT id
                                                        FROM   DeviceModel
                                                        WHERE  BrandId = @BRANDID))
                    BEGIN
                        SET @Message = N'شناسه دستگاه تکراری می باشد';
                        SET @Validate = 0;
                        SET @CodeINSERT = (SELECT Id
                                           FROM   Device
                                           WHERE  Code = @Code
                                                  AND DeviceModelId IN (SELECT id
                                                                        FROM   DeviceModel
                                                                        WHERE  BrandId = @BRANDID));
                    END
					  ELSE
				  IF EXISTS (SELECT TOP(1) 1
                           FROM   [dbo].Device
                           WHERE  IPAddress = @IPAddress AND [Port] = @Port)
                         BEGIN
                   
                        SET @Validate = 0;
                        SET @Id = (SELECT Id
                                           FROM   Device
                                           WHERE IPAddress = @IPAddress AND [Port] = @Port);
							SET @Message =N'آی پی وارد شده با آی پی دستگاه با شماره '+ CAST(@CodeINSERT AS nvarchar(10))+N' یکسان است  '
					
                    END
                ELSE            
                    BEGIN
                        INSERT  INTO [dbo].[Device] ([Code], [DeviceModelID], [Name], [Active], [IPAddress], [Port], [MacAddress], [RegisterDate], [HardwareVersion], [FirmwareVersion], [DeviceLockPassword], [SSL], [TimeSync], [SerialNumber], [DeviceTypeId])
                        VALUES                     (@Code, @DeviceModelId, @Name, @Active, @IPAddress, @Port, @MacAddress, GETDATE(), @HardwareVersion, @FirmwareVersion, @DeviceLockPassword, @SSL, @TimeSync, @SerialNumber, @DeviceTypeId);
                        SET @Id = SCOPE_IDENTITY();
                    END
            END
        ELSE
            BEGIN
                UPDATE [dbo].[Device]
                SET    [DeviceModelId] = @DeviceModelId,
                       [Name]          = @Name,
                       [Code]          = @Code,
                       [Active]        = @Active,
                       [IPAddress]     = @IPAddress,
                       [Port]          = @Port,
                       [TimeSync]      = @TimeSync,
                       [DeviceTypeId]  = @DeviceTypeId,
					   [DeviceLockPassword] = @DeviceLockPassword
                WHERE  Id = @Id;
                SET @CodeInsert = @Id
            END
        COMMIT TRANSACTION T1;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1;
                SET @Validate = 0;
                SET @Message = N'ایجاد/ویرایش انجام نشد';
                SET @CodeInsert = 400
            END
    END CATCH
    SELECT @Message AS [Message],
           @Id AS Id,
           @CodeINSERT AS Code,
           @Validate AS Validate;
END