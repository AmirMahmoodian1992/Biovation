CREATE PROCEDURE [RLY].[InsertEntrance]
@Name nvarchar(50), @Description nvarchar(MAX), @DevicesJson nvarchar(MAX), @SchedulingJson nvarchar(MAX)
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0;
    BEGIN TRY
        BEGIN TRANSACTION T1;
		BEGIN
					INSERT INTO [Rly].[Entrance] ([Name], [Description])
					VALUES (@Name, @Description)
		END
        
        COMMIT TRANSACTION T1;
		SET @Code = SCOPE_IDENTITY();
		IF (@Code > 0)
			BEGIN
				BEGIN TRANSACTION T2;
				BEGIN
					IF EXISTS (SELECT * FROM  
						OPENJSON ( @DevicesJson ) )
							BEGIN
								INSERT INTO [RLY].[EntranceDevice] ([DeviceId], [EntranceId])
								SELECT [DeviceId] , @Code AS [EntranceId]
									FROM OPENJSON(@DevicesJson)
									WITH(
									DeviceId INT '$.Device.DeviceId')
							END
					IF EXISTS (SELECT * FROM  
						OPENJSON ( @SchedulingJson ) )
							BEGIN
								INSERT INTO [RLY].[EntranceScheduling] ([SchedulingId], [EntranceId])
								SELECT SchedulingId , @Code AS [EntranceId]
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
           @Code AS Id,
           @Validate AS Validate;
END
