CREATE PROCEDURE [RLY].[InsertRelayHub]
@IpAddress nvarchar(50), @Port int, @Capacity int, @RelayHubModelId int, @Description nvarchar(MAX)
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
					AND [Capacity] = @Capacity
					AND [RelayHubModelId] = @RelayHubModelId)
				BEGIN
					INSERT INTO [Rly].RelayHub ([IpAddress], [Port], [Capacity], [RelayHubModelId], [Description])
					VALUES (@IpAddress, @Port, @Capacity, @RelayHubModelId, @Description)
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
