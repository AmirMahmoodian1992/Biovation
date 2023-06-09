
CREATE PROCEDURE [dbo].[InsertTask]
@taskTypeCode NVARCHAR (50), @priorityLevelCode NVARCHAR (50), @createdBy INT, @createdAt datetimeoffset(7),@updatedBy INT= null, @updatedAt datetimeoffset(7) = null, @queuedAt datetimeoffset(7) = null, @deviceBrandId INT=NULL, @schedulingPattern nvarchar(50)=null, @dueDate datetimeoffset(7)=null, @json NVARCHAR(MAX)
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (15) = N'201';
    BEGIN TRY
        BEGIN TRANSACTION T1;
		
		CREATE TABLE #TemporaryTaskItem (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TaskItemTypeCode] [nvarchar](50) NOT NULL,
	[PriorityLevelCode] [nvarchar](50) NOT NULL,
	[StatusCode] [nvarchar](50) NOT NULL,
	[DeviceId] [int] NULL,
	[Data] [nvarchar](max) NOT NULL,
	[Result] [nvarchar](max) NULL,
	[OrderIndex] [int] NOT NULL,
	[IsScheduled] [bit] NOT NULL,
	[IsParallelRestricted] [bit] NOT NULL,
	[CurrentIndex] [int] NULL,
	[TotalCount] [int] NULL,
	[ExecutionAt] [datetimeoffset](7) NULL,
	[FinishedAt] [datetimeoffset](7) NULL)


	 IF EXISTS (SELECT * FROM  
                     OPENJSON ( @json ) )
            BEGIN
			 INSERT INTO #TemporaryTaskItem ([TaskItemTypeCode], [PriorityLevelCode], [StatusCode], [DeviceId], [Data], [Result], [OrderIndex], [IsScheduled], [IsParallelRestricted], [CurrentIndex], [TotalCount], [FinishedAt], [ExecutionAt])
                SELECT [TaskItemTypeCode],
                       [PriorityLevelCode],
                       [StatusCode],
                       [DeviceId],
                       [Data],
                       [Result],
                       [OrderIndex],
                       [IsScheduled],
                       [IsParallelRestricted],
                       [CurrentIndex],
                       [TotalCount],
                       [FinishedAt],
                       [ExecutionAt]
                FROM   OPENJSON (@json) WITH (TaskItemTypeCode NVARCHAR (10) '$.TaskItemType.Code', PriorityLevelCode NVARCHAR (10) '$.Priority.Code', StatusCode NVARCHAR (10) '$.Status.Code', DeviceId INT, [Data] NVARCHAR (MAX), Result NVARCHAR (MAX), OrderIndex INT, IsScheduled BIT, IsParallelRestricted BIT, CurrentIndex INT, TotalCount INT, FinishedAt DATETIMEOFFSET(7), ExecutionAt DATETIMEOFFSET(7));
			  IF (SELECT COUNT(*) FROM #TemporaryTaskItem) > 0
			  BEGIN

			  CREATE TABLE #TemporaryInsertedTaskItem(
				[Id] [int] IDENTITY(1,1) NOT NULL,
				[TaskItemTypeCode] [nvarchar](50) NOT NULL,
				[PriorityLevelCode] [nvarchar](50) NOT NULL,
				[StatusCode] [nvarchar](50) NOT NULL,
				[DeviceId] [int] NULL,
				[Data] [nvarchar](max) NOT NULL,
				[Result] [nvarchar](max) NULL,
				[OrderIndex] [int] NOT NULL,
				[IsScheduled] [bit] NOT NULL,
				[IsParallelRestricted] [bit] NOT NULL,
				[CurrentIndex] [int] NULL,
				[TotalCount] [int] NULL,
				[ExecutionAt] [datetimeoffset](7) NULL,
				[FinishedAt] [datetimeoffset](7) NULL)
			   INSERT INTO #TemporaryInsertedTaskItem ([TaskItemTypeCode], [PriorityLevelCode], [StatusCode], [DeviceId], [Data], [Result], [OrderIndex], [IsScheduled], [IsParallelRestricted], [CurrentIndex], [TotalCount], [FinishedAt], [ExecutionAt])
			SELECT [TaskItemTypeCode],
                       [PriorityLevelCode],
                       [StatusCode],
                       [DeviceId],
                       [Data],
                       [Result],
                       [OrderIndex],
                       [IsScheduled],
                       [IsParallelRestricted],
                       [CurrentIndex],
                       [TotalCount],
                       [FinishedAt],
                       [ExecutionAt] FROM #TemporaryTaskItem AS [TTI]
			WHERE NOT EXISTS (
				SELECT * FROM [dbo].TaskItem AS [TI]
				FULL OUTER JOIN [dbo].Task AS [T]
				ON TI.TaskId = T.Id
				WHERE [TI].[TaskItemTypeCode] = [TTI].[TaskItemTypeCode] AND
				[TI].[PriorityLevelCode] >= [TTI].[PriorityLevelCode] AND
				[TI].[StatusCode] IN( 10001,10004,10006) AND
				[TI].[DeviceId] = [TTI].[DeviceId] AND
				[TI].[Data] = [TTI].[Data] AND
				[TI].[OrderIndex] = [TTI].[OrderIndex] AND
				[TI].[IsScheduled] = [TTI].[IsScheduled] AND
				[TI].[IsParallelRestricted] = [TTI].[IsParallelRestricted] AND
				[TI].[CurrentIndex] = [TTI].[CurrentIndex] AND
				[TI].[TotalCount] = [TTI].[TotalCount] AND
				[TI].[FinishedAt] = [TTI].[FinishedAt] AND
				[TI].[ExecutionAt] = [TTI].[ExecutionAt] AND
				[TI].Result IS NULL AND 

				[T].TaskTypeCode = @taskTypeCode AND 
				[T].PriorityLevelCode = @priorityLevelCode AND 
				[T].DeviceBrandId = @deviceBrandId  AND
				ISNULL(SchedulingPattern,'0') = ISNULL(@schedulingPattern,'0')
			)

		IF (SELECT COUNT(*) FROM #TemporaryInsertedTaskItem) >0
		BEGIN
			INSERT  INTO [dbo].[Task] ([TaskTypeCode], [PriorityLevelCode], [CreatedBy], [CreatedAt], [DeviceBrandId], [UpdatedAt], [UpdatedBy], [schedulingPattern], [QueuedAt], [DueDate])
			VALUES    (@taskTypeCode, @priorityLevelCode, @createdBy, @createdAt, @deviceBrandId,@updatedAt,@updatedBy,@schedulingPattern,@queuedAt, @dueDate);
			SET @Code = SCOPE_IDENTITY();
			INSERT INTO TaskItem([TaskId], [TaskItemTypeCode], [PriorityLevelCode],
			 [StatusCode], [DeviceId], [Data], [Result], [OrderIndex], [IsScheduled],
			 [IsParallelRestricted], [CurrentIndex], [TotalCount], [FinishedAt], [ExecutionAt]
			 )
			 SELECT @Code As [TaskId], [TaskItemTypeCode], [PriorityLevelCode],
			 [StatusCode], [DeviceId], [Data], [Result], [OrderIndex], [IsScheduled],
			 [IsParallelRestricted], [CurrentIndex], [TotalCount], [FinishedAt], [ExecutionAt]
			 FROM #TemporaryInsertedTaskItem
		END
		END

  END
        COMMIT TRANSACTION T1;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1;
                SET @Validate = 0;
                SET @Message = N'ایجاد انجام نشد';
                SET @Code = 400;
            END
    END CATCH
    SELECT @Message AS [Message],
           @Code AS Id,
           @Validate AS Validate,
           @Code AS Code;
END
