Create PROCEDURE [dbo].[InsertEntrance]
@Name nvarchar(50), @Code BIGINT,  @Description nvarchar(MAX), @CameraJson nvarchar(MAX), @SchedulingJson nvarchar(MAX)
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @ResultCode AS INT = 0;
    BEGIN TRY
        BEGIN TRANSACTION T1;
		BEGIN
					INSERT INTO [Rly].[Entrance] ([Name], [Code], [Description])
					VALUES (@Name, @Code, @Description)
		END
        
        COMMIT TRANSACTION T1;
		SET @ResultCode = SCOPE_IDENTITY();
		IF (@ResultCode > 0)
			BEGIN
				BEGIN TRANSACTION T2;
				BEGIN
					IF EXISTS (SELECT * FROM  
						OPENJSON ( @CameraJson ) )
							BEGIN
								INSERT INTO [RLY].[EntranceCamera]([CameraId], [EntranceId])
								SELECT [CameraId] , @ResultCode AS [EntranceId]
									FROM OPENJSON(@CameraJson)
									WITH(
									[CameraId] INT '$.Camera.Id')
							END
					IF EXISTS (SELECT * FROM  
						OPENJSON ( @SchedulingJson ) )
							BEGIN
								INSERT INTO [RLY].[EntranceScheduling] ([SchedulingId], [EntranceId])
								SELECT SchedulingId , @ResultCode AS [EntranceId]
									FROM OPENJSON(@SchedulingJson)
									WITH(
									SchedulingId INT '$.Scheduling.Id')
							END
				END
			END
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
