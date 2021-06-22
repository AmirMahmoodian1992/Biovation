CREATE PROCEDURE [dbo].[UpdateEntrance]
@Id INT, @Code BIGINT, @Name nvarchar(50), @Description nvarchar(MAX), @CameraJson nvarchar(MAX), @SchedulingJson nvarchar(MAX)
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @ResultCode AS INT = 0;
    BEGIN TRY
        BEGIN TRANSACTION T1;
		BEGIN
					UPDATE [Rly].[Entrance] 
					SET [Name] = @Name, [Description] = @Description, [Code] = @Code
					WHERE [Id] = @Id
				
		END
        SET @ResultCode = SCOPE_IDENTITY();
        COMMIT TRANSACTION T1;
		BEGIN TRANSACTION T2;
		BEGIN
			IF(@ResultCode > 0 )
			BEGIN
				DELETE FROM [Rly].[EntranceCamera] WHERE [EntranceId] = @Id

				INSERT INTO [Rly].[EntranceCamera] ( [CameraId], [EntranceId])
				SELECT 
					CameraId,
					@Id
					FROM OPENJSON (@CameraJson) WITH (CameraId INT '$.Camera.Id')
			END
		END
		COMMIT TRANSACTION T2;
		BEGIN TRANSACTION T3;
		BEGIN
			IF(@ResultCode > 0 )
			BEGIN
				DELETE FROM [Rly].[EntranceScheduling] WHERE [EntranceId] = @Id

				INSERT INTO [Rly].[EntranceScheduling] ( [SchedulingId], [EntranceId])
				SELECT 
					SchedulingId,
					@Id
					FROM OPENJSON (@SchedulingJson) WITH (SchedulingId INT '$.Scheduling.Id')
			END
		END
		COMMIT TRANSACTION T3;
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
           @ResultCode AS Id,
           @Validate AS Validate;
END
