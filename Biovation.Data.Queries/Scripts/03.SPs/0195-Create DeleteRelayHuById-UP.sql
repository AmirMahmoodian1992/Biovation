CREATE PROCEDURE [dbo].[DeleteRelayHubById]
@Id INT
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'تغییر با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0;
    BEGIN TRY
        BEGIN TRANSACTION T1;
		BEGIN
			IF EXISTS (SELECT Id
				FROM [Rly].RelayHub
				WHERE Id = @Id)
				BEGIN
					DELETE [Rly].RelayHub
					WHERE Id = @Id
                    DELETE [Rly].Relay
                    WHERE RelayHubId = @Id
				END
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
           @Id AS Id,
           @Validate AS Validate;
END