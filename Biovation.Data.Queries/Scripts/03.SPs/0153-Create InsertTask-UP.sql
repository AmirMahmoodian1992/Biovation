
CREATE PROCEDURE [dbo].[InsertTask]
@taskTypeCode NVARCHAR (50), @priorityLevelCode NVARCHAR (50), @createdBy INT, @createdAt datetimeoffset(7),@updatedBy INT= null, @updatedAt datetimeoffset(7) = null, @queuedAt datetimeoffset(7) = null, @deviceBrandId INT=NULL, @schedulingPattern nvarchar(50)=null, @dueDate datetimeoffset(7)=null, @json NVARCHAR(MAX)
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (5) = N'201';
    BEGIN TRY
        BEGIN TRANSACTION T1;
        INSERT  INTO [dbo].[Task] ([TaskTypeCode], [PriorityLevelCode], [CreatedBy], [CreatedAt], [DeviceBrandId], [UpdatedAt], [UpdatedBy], [schedulingPattern], [QueuedAt], [DueDate])
        VALUES    (@taskTypeCode, @priorityLevelCode, @createdBy, @createdAt, @deviceBrandId,@updatedAt,@updatedBy,@schedulingPattern,@queuedAt, @dueDate);
        SET @Code = SCOPE_IDENTITY();
        IF EXISTS (SELECT * FROM  
                     OPENJSON ( @json ) )
            BEGIN

 INSERT INTO TaskItem([TaskId], [TaskItemTypeCode], [PriorityLevelCode],
 [StatusCode], [DeviceId], [Data], [Result], [OrderIndex], [IsScheduled],
 [IsParallelRestricted], [CurrentIndex], [TotalCount], [FinishedAt], [ExecutionAt]
 )
 SELECT @Code As [TaskId], [TaskItemTypeCode], [PriorityLevelCode],
 [StatusCode], [DeviceId], [Data], [Result], [OrderIndex], [IsScheduled],
 [IsParallelRestricted], [CurrentIndex], [TotalCount], [FinishedAt], [ExecutionAt]
 FROM OPENJSON(@json)
 WITH (
	TaskItemTypeCode nvarchar(10) '$.TaskItemType.Code',
	PriorityLevelCode nvarchar(10) '$.Priority.Code',
	StatusCode nvarchar(10) '$.Status.Code',
	DeviceId int,
	Data nvarchar(MAX),
	Result nvarchar(MAX),
	OrderIndex int ,
	IsScheduled bit ,
	IsParallelRestricted bit,
	CurrentIndex int,
	TotalCount int ,
	FinishedAt datetimeoffset,
	ExecutionAt datetimeoffset
	   )

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
GO
