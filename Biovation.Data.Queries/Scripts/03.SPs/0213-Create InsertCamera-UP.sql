CREATE PROCEDURE [dbo].[InsertCamera]
@Code INT, @Name NVARCHAR (MAX)= NULL,@Ip NVARCHAR (50), @Port INT, @UserName NVARCHAR (MAX) ,@Password NVARCHAR (MAX), @MacAddress NVARCHAR (MAX)= NULL, @BrandCode INT, @Description NVARCHAR (MAX)= NULL, @Active BIT, @HardwareVersion NVARCHAR (MAX)= NULL, @SerialNumber NVARCHAR (MAX)= NULL, @ConnectionUrl NVARCHAR (MAX)= NULL, @LiveStreamUrl NVARCHAR (MAX)= NULL, @ModelId INT
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ثبت با موفقیت انجام گرفت', @Validate AS INT = 1, @CameraCode AS INT = 0, @CameraId AS INT = 0;
    BEGIN TRY
        BEGIN TRANSACTION T1;
        BEGIN
            IF NOT EXISTS (SELECT ID
                           FROM   [dbo].[Camera]
                           WHERE  [Code] = @Code
                                  AND [ModelId] = @ModelId
                                  AND [BrandCode] = @BrandCode)
                BEGIN
                    INSERT  INTO [dbo].[Camera] ([Code], [Name], [Ip], [Port], [UserName], [Password], [MacAddress], [BrandCode], [Description], [Active], [RegisterDate], [HardwareVersion], [SerialNumber], [ConnectionUrl], [LiveStreamUrl], [ModelId])
                    VALUES              (@Code, @Name, @Ip, @Port, @UserName, @Password, @MacAddress, @BrandCode, @Description, @Active, GETDATE(), @HardwareVersion, @SerialNumber, @ConnectionUrl, @LiveStreamUrl, @ModelId);
                    SET @CameraCode = SCOPE_IDENTITY();
                END
			ELSE
				BEGIN
					SET @Message = N'به علت وجود داده تکراری ثبت انجام نشد';
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
           @CameraCode AS Id,
           @Validate AS Validate;
END