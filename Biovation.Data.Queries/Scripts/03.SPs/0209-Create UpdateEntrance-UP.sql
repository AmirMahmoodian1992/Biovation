CREATE PROCEDURE [dbo].[UpdateEntrance]
--declare
@Id INT, @Code BIGINT, @Name NVARCHAR (50), @Description NVARCHAR (MAX) = null, @CamerasJson NVARCHAR (MAX) = null, @SchedulingsJson NVARCHAR (MAX) = null, @DevicesJson nvarchar(MAX) = null
AS


BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @ResultCode AS INT = 0;
    BEGIN TRY
        BEGIN TRANSACTION T1;
        BEGIN
            UPDATE [Rly].[Entrance]
            SET    [Name]        = @Name,
                   [Description] = @Description,
                   [Code]        = @Code
            WHERE  [Id] = @Id;

			DELETE [Rly].[EntranceCamera]
                    WHERE  [EntranceId] = @Id;
                    IF (@CamerasJson is not null and  EXISTS (SELECT *
                               FROM   OPENJSON (@CamerasJson)))
                        BEGIN
                            INSERT INTO [Rly].[EntranceCamera] ([CameraId], [EntranceId])
                            SELECT CameraId,
                                   @Id
                            FROM   OPENJSON (@CamerasJson) WITH (CameraId INT '$.Id');
                        END
						DELETE [Rly].[EntranceScheduling]
						WHERE  [EntranceId] = @Id;
						IF (@SchedulingsJson is not null and EXISTS (SELECT *
								   FROM   OPENJSON (@SchedulingsJson)))
							BEGIN
								INSERT INTO [Rly].[EntranceScheduling] ([SchedulingId], [EntranceId])
								SELECT SchedulingId,
									   @Id
								FROM   OPENJSON (@SchedulingsJson) WITH (SchedulingId INT '$.Id');
							END

						DELETE [Rly].[EntranceDevice]
						WHERE  [EntranceId] = @Id;
						IF (@DevicesJson is not null and EXISTS (SELECT *
								   FROM   OPENJSON (@DevicesJson)))
							BEGIN
								INSERT INTO [Rly].[EntranceDevice] ([DeviceId], [EntranceId])
								SELECT DeviceId,
									   @Id
								FROM   OPENJSON (@DevicesJson) WITH (DeviceId INT '$.DeviceId');
							END
        END
        
		COMMIT TRANSACTION T1;

    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1;
				--select ERROR_LINE()
				--select  ERROR_MESSAGE()
                SET @Validate = 0;
                SET @Message = N'ثبت انجام نشد';
            END
    END CATCH
    SELECT @Message AS [Message],
           @ResultCode AS Id,
           @Validate AS Validate;
END