CREATE PROCEDURE [RLY].[UpdateRelay]
@Id INT, @Name NVARCHAR (MAX), @NodeNumber INT, @RelayHubId INT, @EntranceId INT, @Description  NVARCHAR (MAX), @json  NVARCHAR (MAX)
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0;
    BEGIN TRY
        BEGIN TRANSACTION T1;
		BEGIN
					UPDATE [Rly].[Relay] 
					SET [Name] = @Name, [Description] = @Description, [NodeNumber] = @NodeNumber, [RelayHubId] = @RelayHubId, [EntranceId] = @RelayHubId
					WHERE [Id] = @Id
				
		END
        SET @Code = SCOPE_IDENTITY();
        COMMIT TRANSACTION T1;
		BEGIN TRANSACTION T2;
		BEGIN
			
			DELETE FROM [Rly].[RelayScheduling] WHERE [RelayId] = @Id

			INSERT INTO [Rly].[RelayScheduling] ( [RelayId], [SchedulingId])
			SELECT 
				@Code,
				SchedulingId
				FROM OPENJSON (@json) WITH (SchedulingId INT '$.Scheduling.Id')
			
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
