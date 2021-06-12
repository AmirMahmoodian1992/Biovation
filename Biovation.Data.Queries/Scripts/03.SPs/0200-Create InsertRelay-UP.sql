CREATE PROCEDURE [RLY].[InsertRelay]
@Name NVARCHAR (MAX), @NodeNumber INT, @RelayHubId INT, @EntranceId INT, @Description  NVARCHAR (MAX), @json  NVARCHAR (MAX)
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0;
    BEGIN TRY
        BEGIN TRANSACTION T1;
		BEGIN
					INSERT INTO [Rly].[Relay] ([Name], [Description], [NodeNumber], [RelayHubId], [EntranceId])
					VALUES (@Name, @Description, @NodeNumber, @RelayHubId, @EntranceId)
					
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
					FROM OPENJSON (@json) WITH (SchedulingId INT '$.Scheduling.Id')
			END
		END
		COMMIT TRANSACTION T2;
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
