CREATE PROCEDURE [dbo].[UpdateRelayHub]
@Id INT,@IpAddress nvarchar(50), @Port int, @Name NVARCHAR(MAX), @Active BIT, @Capacity int, @RelayHubModelId int, @Description nvarchar(MAX) = null
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'تغییر با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0;
    BEGIN TRY
        BEGIN TRANSACTION T1;
		BEGIN
			UPDATE [Rly].[RelayHub]
			SET [IpAddress] = @IpAddress, [Port] = @Port, [Name] = @Name, [Active] = @Active, [Capacity] = @Capacity, [RelayHubModelId] = @RelayHubModelId, [Description] = @Description
			WHERE [Id] = @Id
		END
        SET @Code = SCOPE_IDENTITY();
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
           @Code AS Id,
           @Validate AS Validate;
END
