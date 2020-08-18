
CREATE PROCEDURE [dbo].[InsertTask]
@taskTypeCode nvarchar(50), @priorityLevelCode nvarchar(50), @createdBy INT, @createdAt datetime, @deviceBrandId INT = NULL, @taskItems TaskItemTable readonly
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0
    BEGIN TRY
        BEGIN TRANSACTION T1
            INSERT  INTO [dbo].[Task] ([TaskTypeCode], [PriorityLevelCode], [CreatedBy], [CreatedAt], [DeviceBrandId])
            VALUES                          (@taskTypeCode, @priorityLevelCode, @createdBy, @createdAt, @deviceBrandId)
            SET @Code = SCOPE_IDENTITY()

			IF EXISTS (SELECT * FROM @taskItems)
			BEGIN
						WITH Src AS(
						SELECT @Code AS [TaskId], [TaskItemTypeCode], [PriorityLevelCode], [StatusCode], [DeviceId], [Data], [Result], [OrderIndex], [IsScheduled], [DueDate], [IsParallelRestricted] 
							FROM @taskItems ), taskItems AS (SELECT * FROM [TaskItem] AS [TI] )


						MERGE taskItems AS T
						USING (SELECT [TaskId], [TaskItemTypeCode], [PriorityLevelCode], [StatusCode], [DeviceId], [Data], [Result], [OrderIndex], [IsScheduled], [DueDate], [IsParallelRestricted] from src
						) as S
						ON (T.[TaskId] = S.[TaskId] AND t.[TaskItemTypeCode]=s.[TaskItemTypeCode] AND t.[DeviceId]=s.[DeviceId] AND s.[DueDate] = t.[DueDate]
						)
						WHEN NOT MATCHED BY Target
						THEN INSERT([TaskId], [TaskItemTypeCode], [PriorityLevelCode], [StatusCode], [DeviceId], [Data], [Result], [OrderIndex], [IsScheduled], [DueDate], [IsParallelRestricted]) 
						VALUES ([TaskId], [TaskItemTypeCode], [PriorityLevelCode], [StatusCode], [DeviceId], [Data], [Result], [OrderIndex], [IsScheduled], [DueDate], [IsParallelRestricted]); 
			END
        COMMIT TRANSACTION T1
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1
                SET @Validate = 0
                SET @Message = N'ایجاد انجام نشد'
            END
    END CATCH
    SELECT @Message AS [Message],
           @Code AS Id,
           @Validate AS Validate
END
GO
