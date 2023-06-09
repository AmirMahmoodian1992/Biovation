
CREATE PROCEDURE [dbo].[InsertLogBulk]
@values VARCHAR(MAX)
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ثبت با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0
    BEGIN TRY
        BEGIN TRANSACTION T1
        BEGIN
	
			CREATE TABLE #TEMP_LOGS(
				[Id] [int] IDENTITY(1,1) NOT NULL,
				[DeviceId] [bigint] NOT NULL,
				[EventId] [int] NOT NULL,
				[UserId] [int] NOT NULL,
				[DateTime] [datetime] NOT NULL,
				[Ticks] [bigint] NOT NULL,
				[SubEvent] [int] NOT NULL,
				[TNAEvent] [int] NOT NULL,
				[InOutMode] [int] NOT NULL,
				[MatchingType] [int] NOT NULL,
				[SuccessTransfer] [bit] NULL)
		exec ('	

			--SELECT @values

			INSERT INTO #TEMP_LOGS (
			[DeviceId]
           ,[EventId]
           ,[UserId]
           ,[DateTime]
           ,[Ticks]
           ,[SubEvent]
           ,[TNAEvent]
		   ,[InOutMode]
           ,[MatchingType]
           ,[SuccessTransfer]) VALUES'+ @values +'


			--SELECT * FROM #TEMP_LOGS

			')

			INSERT INTO [Log]
			([DeviceId]
            ,[EventId]
            ,[UserId]
            ,[DateTime]
            ,[Ticks]
            ,[SubEvent]
            ,[TNAEvent]
			,[InOutMode]
            ,[MatchingType]
            ,[SuccessTransfer])
				SELECT DISTINCT [DeviceId]
							   ,[EventId]
							   ,[UserId]
							   ,[DateTime]
							   ,[Ticks]
							   ,[SubEvent]
							   ,[TNAEvent]
							   ,[InOutMode]
							   ,[MatchingType]
							   ,[SuccessTransfer]
				FROM #TEMP_LOGS AS T
				WHERE NOT EXISTS (
				SELECT  *
				FROM [Log] As L
				WHERE T.[DateTime] = L.[DateTime] AND T.[DeviceId] = L.[DeviceId] AND T.[EventId] = L.[EventId])
	


			DROP TABLE #TEMP_LOGS
        END
        SET @Code = SCOPE_IDENTITY()
        COMMIT TRANSACTION T1
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1
                SET @Validate = 0
                SET @Message = N'ثبت انجام نشد'
            END
    END CATCH
    SELECT @Message AS [Message],
           @Code AS Id,
           @Validate AS Validate
END