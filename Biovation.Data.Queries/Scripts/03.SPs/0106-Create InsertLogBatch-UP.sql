ALTER PROCEDURE [dbo].[InsertLogBatch]
@LogTable LogTable readonly
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ثبت با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0
    BEGIN TRY
        BEGIN TRANSACTION T1
        BEGIN

						--WITH Src
						--AS (SELECT d.Id AS DeviceId,
						-- [EventId],
						-- [UserId],
						-- [DateTime],
						-- [Ticks],
						-- [SubEvent],
						-- [TNAEvent],
						-- [MatchingType],
						-- [SuccessTransfer]
						-- FROM @LogTable AS l
						-- INNER JOIN
						-- Device AS d
						-- ON l.DeviceCode = d.Code
						-- AND DeviceModelId IN (SELECT Id
						-- FROM DeviceModel
						-- WHERE Id = d.DeviceModelId)),
						-- a
						--AS (SELECT *
						-- FROM [Log] AS l)
						--MERGE INTO a
						-- AS T
						--USING (SELECT DeviceId,
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
						--WHEN NOT MATCHED BY TARGET THEN INSERT ([DeviceId], [EventId], [UserId], [DateTime], [Ticks], [SubEvent], [TNAEvent], [MatchingType], [SuccessTransfer], [CreateAt]) VALUES ([DeviceId], [EventId], [UserId], [DateTime], [Ticks], [SubEvent], [TNAEvent], [MatchingType], [SuccessTransfer], GETDATE());


						INSERT [dbo].[Log] ([DeviceId], [EventId], [UserId], [DateTime], [Ticks], [SubEvent], [TNAEvent], [InOutMode], [MatchingType], [SuccessTransfer], [CreateAt]) 
						SELECT DISTINCT
							   [TL].[DeviceId],
							   [TL].[EventId],
							   [TL].[UserId],
							   [TL].[DateTime],
							   [TL].[Ticks],
							   [TL].[SubEvent],
							   [TL].[TNAEvent],
							   [TL].[InOutMode],
							   [TL].[MatchingType],
							   [TL].[SuccessTransfer],
							   GETDATE()
						FROM   [dbo].[Log] AS L
								RIGHT JOIN
								@LogTable AS TL
								ON L.DeviceId = TL.DeviceId
									AND L.EventId = TL.EventId
									AND L.UserId = TL.UserId
									AND L.[DateTime] = TL.[DateTime]
									WHERE L.[Id] IS NULL


					SELECT @Code = cast(@@ROWCOUNT as nvarchar);;

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