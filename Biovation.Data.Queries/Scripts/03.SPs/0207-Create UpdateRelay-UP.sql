CREATE PROCEDURE [dbo].[UpdateRelay]
@Id INT, @Name NVARCHAR (MAX), @NodeNumber INT, @RelayHubId INT, @RelayTypeId INT, @Description  NVARCHAR (MAX), @SchedulingsJson  NVARCHAR (MAX) = NULL, @DevicesJson  NVARCHAR (MAX) = NULL, @CamerasJson  NVARCHAR (MAX) = NULL
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'تغییر با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0;
    BEGIN TRY
        BEGIN TRANSACTION T1;
		BEGIN
				DECLARE @PreviousRelayId INT;
				SELECT @PreviousRelayId = [Id] FROM [Rly].Relay WHERE [RelayHubId] = @RelayHubId AND [NodeNumber] = @NodeNumber
				DELETE FROM [Rly].RelayDevice WHERE [RelayId] = @PreviousRelayId
				DELETE FROM [Rly].RelayCamera WHERE [RelayId] = @PreviousRelayId
				DELETE FROM [Rly].RelayScheduling WHERE [RelayId] = @PreviousRelayId
				DELETE FROM [Rly].Relay WHERE [Id] = @PreviousRelayId
				INSERT INTO [Rly].[Relay] ([Name], [Description], [NodeNumber], [RelayHubId], [RelayTypeId])
				VALUES (@Name, @Description, @NodeNumber, @RelayHubId,@RelayTypeId)
				
		END
		COMMIT TRANSACTION T1;
        SET @Code = SCOPE_IDENTITY();

		BEGIN TRANSACTION T2;
		BEGIN

			--DELETE FROM [Rly].[RelayScheduling] WHERE [RelayId] = @Id
			IF (@SchedulingsJson is not null and  EXISTS (SELECT *
                               FROM   OPENJSON (@SchedulingsJson)))
			BEGIN
				INSERT INTO [Rly].[RelayScheduling] ( [RelayId], [SchedulingId])
				SELECT 
					@Code,
					SchedulingId
					FROM OPENJSON (@SchedulingsJson) WITH (SchedulingId INT '$.Id')
			END
		
			
			--DELETE FROM [Rly].[RelayDevice] WHERE [RelayId] = @Id
			 IF (@DevicesJson is not null and  EXISTS (SELECT *
                               FROM   OPENJSON (@DevicesJson)))
			BEGIN

				INSERT INTO [Rly].[RelayDevice] ( [RelayId], [DeviceId])
				SELECT 
					@Code,
					DeviceId
					FROM OPENJSON (@DevicesJson) WITH (DeviceId INT '$.DeviceId')
			END


		
			--DELETE FROM [Rly].[RelayCamera] WHERE [RelayId] = @Id
			  IF (@CamerasJson is not null and  EXISTS (SELECT *
                               FROM   OPENJSON (@CamerasJson)))
			BEGIN

				INSERT INTO [Rly].[RelayCamera] ( [RelayId], [CameraId])
				SELECT 
					@Code,
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
