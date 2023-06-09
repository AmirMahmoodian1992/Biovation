CREATE PROCEDURE [dbo].[InsertRelayHub]
@IpAddress nvarchar(50), @Port int, @Name nvarchar(MAX), @Active bit, @Capacity int, @RelayHubModelId int, @Description nvarchar(MAX)
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0;
    BEGIN TRY
        BEGIN TRANSACTION T1;
		BEGIN
			IF NOT EXISTS (SELECT ID 
				FROM [Rly].RelayHub
				WHERE [IpAddress] = @IpAddress
					AND [Port] = @Port
					AND [Name] = @Name
					AND [RelayHubModelId] = @RelayHubModelId)
				BEGIN
					INSERT INTO [Rly].RelayHub ([IpAddress], [Port], [Name], [Active], [Capacity], [RelayHubModelId], [Description])
					VALUES (@IpAddress, @Port, @Name, @Active, @Capacity, @RelayHubModelId, @Description)
				END
            ELSE
                BEGIN
                SET @Message = N'به علت وجود داده تکراری ثبت انجام نشد';
                END
		END
        SET @Code = SCOPE_IDENTITY();
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
           @Validate AS Validate;
END
