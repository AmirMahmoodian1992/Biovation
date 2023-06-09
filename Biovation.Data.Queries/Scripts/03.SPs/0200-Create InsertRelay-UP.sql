CREATE PROCEDURE [dbo].[InsertRelay]
@Name NVARCHAR (MAX), @NodeNumber INT, @RelayHubId INT, @RelayTypeId INT, @Description  NVARCHAR (MAX), @SchedulingsJson  NVARCHAR (MAX)= NULL, @DevicesJson NVARCHAR (MAX)= NULL, @CamerasJson NVARCHAR (MAX)= NULL
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0;
    BEGIN TRY
        BEGIN TRANSACTION T1;
		BEGIN
					INSERT INTO [Rly].[Relay] ([Name], [Description], [NodeNumber], [RelayHubId], [RelayTypeId])
					VALUES (@Name, @Description, @NodeNumber, @RelayHubId, @RelayTypeId)
					
		END
        SET @Code = SCOPE_IDENTITY();
        COMMIT TRANSACTION T1;
		BEGIN TRANSACTION T2;
		BEGIN
			IF(@Code > 0)
			BEGIN
				INSERT INTO [Rly].[RelayScheduling] ( [RelayId], [SchedulingId])
				SELECT 
					@Code,
					SchedulingId
					FROM OPENJSON (@SchedulingsJson) WITH (SchedulingId INT '$.Id')
			END
		END
		BEGIN TRANSACTION T3;
		BEGIN
			IF(@Code > 0)
			BEGIN
				INSERT INTO [Rly].[RelayDevice] ( [RelayId], [DeviceId])
				SELECT 
					@Code,
					DeviceId
					FROM OPENJSON (@DevicesJson) WITH (DeviceId INT '$.DeviceId')
			END
		END
		BEGIN TRANSACTION T4;
		BEGIN
			IF(@Code > 0)
			BEGIN
				INSERT INTO [Rly].[RelayCamera] ( [RelayId], [CameraId])
				SELECT 
					@Code,
					CameraId
					FROM OPENJSON (@CamerasJson) WITH (CameraId INT '$.Id')
			END
		END
		COMMIT TRANSACTION T2;
		COMMIT TRANSACTION T3;
		COMMIT TRANSACTION T4;
    END TRY	
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1;
                ROLLBACK TRANSACTION T2;
                ROLLBACK TRANSACTION T3;
                ROLLBACK TRANSACTION T4;
                SET @Validate = 0;
                SET @Message = N'ثبت انجام نشد';
            END
    END CATCH
    SELECT @Message AS [Message],
           @Code AS Id,
           @Validate AS Validate;
END
