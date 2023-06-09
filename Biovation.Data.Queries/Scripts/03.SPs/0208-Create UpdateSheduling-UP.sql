CREATE PROCEDURE [dbo].[UpdateSheduling]
@Id INT,@StartTime TIME, @EndTime TIME, @Mode NVARCHAR(10)
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'تغییر با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0;
    BEGIN TRY
        BEGIN TRANSACTION T1;
		BEGIN
			UPDATE [Rly].[Sheduling]
			SET [StartTime] = @StartTime, [EndTime] = @EndTime, [Mode] = @Mode
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
