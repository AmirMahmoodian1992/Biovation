CREATE PROCEDURE [dbo].[UpdateRelay]
@Id INT, @Name NVARCHAR (MAX), @NodeNumber INT, @RelayHubId INT, @RelayTypeId INT, @Description  NVARCHAR (MAX), @SchedulingsJson  NVARCHAR (MAX) = NULL, @DevicesJson  NVARCHAR (MAX) = NULL, @CamerasJson  NVARCHAR (MAX) = NULL
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'تغییر با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0;
    BEGIN TRY
        BEGIN TRANSACTION T1;
		BEGIN
				IF NOT EXISTS (SELECT [Id]
					FROM [Rly].[Relay]
					WHERE [Id] = @Id)
					BEGIN
						UPDATE [Rly].[Relay] 
						SET [Name] = @Name, [Description] = @Description, [NodeNumber] = @NodeNumber, [RelayHubId] = @RelayHubId, [RelayTypeId] = @RelayTypeId
						WHERE [Id] = @Id
					END
				ELSE
					BEGIN
						INSERT INTO [Rly].[Relay] ([Name], [Description], [NodeNumber], [RelayHubId])
						VALUES (@Name, @Description, @NodeNumber, @RelayHubId)
					END
				
		END
        SET @Code = SCOPE_IDENTITY();
        COMMIT TRANSACTION T1;
		BEGIN TRANSACTION T2;
		BEGIN

			DELETE FROM [Rly].[RelayScheduling] WHERE [RelayId] = @Id
			IF (@SchedulingsJson is not null and  EXISTS (SELECT *
                               FROM   OPENJSON (@SchedulingsJson)))
			BEGIN
				INSERT INTO [Rly].[RelayScheduling] ( [RelayId], [SchedulingId])
				SELECT 
					@Id,
					SchedulingId
					FROM OPENJSON (@SchedulingsJson) WITH (SchedulingId INT '$.Scheduling.Id')
			END
		
			
			DELETE FROM [Rly].[RelayDevice] WHERE [RelayId] = @Id
			 IF (@DevicesJson is not null and  EXISTS (SELECT *
                               FROM   OPENJSON (@DevicesJson)))
			BEGIN

				INSERT INTO [Rly].[RelayDevice] ( [RelayId], [DeviceId])
				SELECT 
					@Id,
					DeviceId
					FROM OPENJSON (@DevicesJson) WITH (DeviceId INT '$.Devices.DeviceId')
			END
		
			DELETE FROM [Rly].[RelayCamera] WHERE [RelayId] = @Id
			  IF (@CamerasJson is not null and  EXISTS (SELECT *
                               FROM   OPENJSON (@CamerasJson)))
			BEGIN

				INSERT INTO [Rly].[RelayCamera] ( [RelayId], [CameraId])
				SELECT 
					@Id,
					CameraId
					FROM OPENJSON (@CamerasJson) WITH (CameraId INT '$.Id')
			END
		END
		COMMIT TRANSACTION T2;
		
    END TRY	
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1;
                ROLLBACK TRANSACTION T2;
                SET @Validate = 0;
                SET @Message = N'ثبت انجام نشد';
            END
    END CATCH
    SELECT @Message AS [Message],
           @Code AS Id,
           @Validate AS Validate;
END
