ALTER PROCEDURE [dbo].[SetLogIsTransferedBatch]
@LogTable LogTable readonly
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ثبت با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0
    BEGIN TRY
        BEGIN TRANSACTION T1
        BEGIN

						--WITH Src
						--AS (SELECT [DeviceId],
						-- [EventId],
						-- [UserId],
						-- [DateTime],
						-- [Ticks],
						-- [SubEvent],
						-- [TNAEvent],
						-- [MatchingType],
						-- [SuccessTransfer]
						-- FROM @LogTable),
						-- a
						--AS (SELECT *
						-- FROM [Log] AS l)
						--MERGE INTO a
						-- AS T
						--USING (SELECT DISTINCT DeviceId,
						-- [EventId],
						-- [UserId],
						-- [DateTime],
						-- [Ticks],
						-- [SubEvent],
						-- [TNAEvent],
						-- [MatchingType],
						-- [SuccessTransfer]
						-- FROM src) AS S ON (T.DeviceId = S.DeviceId
						-- AND t.EventId = s.EventId
						-- AND t.UserId = s.UserId
						-- AND s.Ticks = t.Ticks)
						--WHEN MATCHED THEN UPDATE 
						--SET T.SuccessTransfer = 1;

						UPDATE [dbo].[Log] SET [SuccessTransfer] = 1 
						WHERE [Id] IN (SELECT L.[Id] FROM [dbo].[Log] AS L JOIN @LogTable AS TL 
						ON L.DeviceId = TL.DeviceId
						AND L.EventId = TL.EventId
						AND L.UserId = TL.UserId
						AND L.[DateTime] = TL.[DateTime])
						AND [SuccessTransfer] = 0


					SELECT @Code = cast(@@ROWCOUNT as nvarchar);

        -- Insert into dbo.[Log]
		  --SELECT  FROM @LogTable
        END
        --SET @Code = SCOPE_IDENTITY()
        COMMIT TRANSACTION T1
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN

                ROLLBACK TRANSACTION T1
				--select ERROR_MESSAGE()
                SET @Validate = 0
                SET @Message = N'ثبت انجام نشد'
            END
    END CATCH
    SELECT @Message AS [Message],
           @Code AS Id,
           @Validate AS Validate
END