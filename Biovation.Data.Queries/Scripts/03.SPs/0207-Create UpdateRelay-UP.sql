CREATE PROCEDURE [dbo].[UpdateRelay]
@Id INT, @Name NVARCHAR (MAX), @NodeNumber INT, @RelayHubId INT, @EntranceId INT, @Description  NVARCHAR (MAX), @SchedulingsJson  NVARCHAR (MAX), @DevicesJson  NVARCHAR (MAX)
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0;
    BEGIN TRY
        BEGIN TRANSACTION T1;
		BEGIN
					UPDATE [Rly].[Relay] 
					SET [Name] = @Name, [Description] = @Description, [NodeNumber] = @NodeNumber, [RelayHubId] = @RelayHubId, [EntranceId] = @EntranceId
					WHERE [Id] = @Id
				
		END
        SET @Code = SCOPE_IDENTITY();
        COMMIT TRANSACTION T1;
		BEGIN TRANSACTION T2;
		BEGIN
			
			IF (@Code > 0)
			BEGIN
				DELETE FROM [Rly].[RelayScheduling] WHERE [RelayId] = @Id

				INSERT INTO [Rly].[RelayScheduling] ( [RelayId], [SchedulingId])
				SELECT 
					@Id,
					SchedulingId
					FROM OPENJSON (@SchedulingsJson) WITH (SchedulingId INT '$.Scheduling.Id')
			END
		END
		COMMIT TRANSACTION T2;
		BEGIN TRANSACTION T3;
		BEGIN
			
			IF (@Code > 0)
			BEGIN
				DELETE FROM [Rly].[RelayDevice] WHERE [RelayId] = @Id

				INSERT INTO [Rly].[RelayDevice] ( [RelayId], [DeviceId])
				SELECT 
					@Id,
					DeviceId
					FROM OPENJSON (@DevicesJson) WITH (DeviceId INT '$.Devices.DeviceId')
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
           @Code AS Id,
           @Validate AS Validate;
END
