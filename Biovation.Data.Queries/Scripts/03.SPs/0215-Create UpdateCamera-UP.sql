CREATE PROCEDURE [dbo].[UpdateCamera]
@Id BIGINT, @Code INT, @Name NVARCHAR (MAX)= NULL,@Ip NVARCHAR (50), @Port INT, @UserName NVARCHAR (MAX) ,@Password NVARCHAR (MAX), @MacAddress NVARCHAR (MAX)= NULL, @BrandCode INT, @ResolutionCode INT, @Description NVARCHAR (MAX)= NULL, @Active BIT, @HardwareVersion NVARCHAR (MAX)= NULL, @SerialNumber NVARCHAR (MAX)= NULL, @ConnectionUrl NVARCHAR (MAX)= NULL, @LiveStreamUrl NVARCHAR (MAX)= NULL, @ModelId INT
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N' تغییر با موفقیت انجام گرفت', @Validate AS INT = 1, @CameraCode AS INT = 0, @CameraId AS INT = 0;
    BEGIN TRY
        BEGIN TRANSACTION T1;
        BEGIN
           
                    UPDATE [Rly].[Camera] 
				SET [Code] = @Code, 
					[Name] = @Name, 
					[Ip] = @Ip, 
					[Port] = @Port, 
					[UserName] = @UserName, 
					[Password] = @Password, 
					[MacAddress] = @MacAddress, 
					[BrandCode] = @BrandCode, 
					[ResolutionCode] = @ResolutionCode, 
					[Description] = @Description, 
					[Active] = @Active, 
					[HardwareVersion] = @HardwareVersion, 
					[SerialNumber] = @SerialNumber, 
					[ConnectionUrl] = @ConnectionUrl, 
					[LiveStreamUrl] = @LiveStreamUrl, 
					[ModelId] = @ModelId
					WHERE  [Id] = @Id
                    
                
        END
        COMMIT TRANSACTION T1;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1;
                SET @Validate = 0;
                SET @Message = N'تغییر انجام نشد';
            END
    END CATCH

    SELECT @Message AS [Message],
           @CameraCode AS Id,
           @Validate AS Validate;
END