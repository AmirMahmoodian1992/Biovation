CREATE PROCEDURE [dbo].[InsertSheduling]
@StartTime TIME, @EndTime TIME, @Mode NVARCHAR(10)
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0;
    BEGIN TRY
        BEGIN TRANSACTION T1;
		BEGIN
			IF NOT EXISTS (SELECT Id
				FROM [Rly].[Sheduling]
				WHERE [StartTime] = @StartTime
					AND [EndTime] = @EndTime
					AND [Mode] = @Mode)
				BEGIN
					INSERT INTO [Rly].[Sheduling] ([StartTime], [EndTime], [Mode])
					VALUES (@StartTime,@EndTime,@Mode)
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
