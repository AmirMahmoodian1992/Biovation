CREATE PROCEDURE [dbo].[UpdateEntrance]
@Id INT, @Code BIGINT, @Name NVARCHAR (50), @Description NVARCHAR (MAX), @CamerasJson NVARCHAR (MAX), @SchedulingsJson NVARCHAR (MAX)
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
        END
        SET @ResultCode = SCOPE_IDENTITY();
        BEGIN TRANSACTION T2;
        BEGIN
                    DELETE [Rly].[EntranceCamera]
                    WHERE  [EntranceId] = @Id;
                    IF EXISTS (SELECT *
                               FROM   OPENJSON (@CamerasJson))
                        BEGIN
                            INSERT INTO [Rly].[EntranceCamera] ([CameraId], [EntranceId])
                            SELECT CameraId,
                                   @Id
                            FROM   OPENJSON (@CamerasJson) WITH (CameraId INT '$.Id');
                        END
               
        END
        BEGIN TRANSACTION T3;
        BEGIN
           
                    DELETE [Rly].[EntranceScheduling]
                    WHERE  [EntranceId] = @Id;
                    IF EXISTS (SELECT *
                               FROM   OPENJSON (@SchedulingsJson))
                        BEGIN
                            INSERT INTO [Rly].[EntranceScheduling] ([SchedulingId], [EntranceId])
                            SELECT SchedulingId,
                                   @Id
                            FROM   OPENJSON (@SchedulingsJson) WITH (SchedulingId INT '$.Id');
                        END
                
        END
		COMMIT TRANSACTION T1;
		COMMIT TRANSACTION T2;
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