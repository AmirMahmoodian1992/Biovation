
CREATE PROCEDURE [dbo].[UpdateTaskItemStatus]
@Id INT,
@statusCode NVARCHAR (50),
@CurrentIndex INT=null,
@TotalCount INT=null,
@result NVARCHAR (MAX)=NULL
AS
BEGIN  
   DECLARE @Message AS NVARCHAR (200) = N'ویرایش با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (15) = N'201'
     ,@FinishedAt DATETIMEOFFSET(7)=null, @ExecutionAt DATETIMEOFFSET(7)=null;
	BEGIN TRY
        BEGIN TRANSACTION T1;
        BEGIN
            IF EXISTS (SELECT Id
                       FROM   [dbo].[TaskItem]
                       WHERE  Id = @Id)
                BEGIN
			
                    UPDATE [dbo].[TaskItem]
                    SET    [StatusCode] = @statusCode,
                           [Result]     = @result,
						   [CurrentIndex] = IsNull(@CurrentIndex, CurrentIndex),
						   [TotalCount] = IsNull(@TotalCount,TotalCount),
						   [ExecutionAt] =CASE  
                        WHEN @statusCode = 10001 and ExecutionAt = null THEN GETDATE()                  
                        ELSE null
						END,
						[FinishedAt] =CASE  
                        WHEN @statusCode = 10003 and FinishedAt = null THEN GETDATE()                 
                        ELSE null
                        END 						   
                    WHERE  Id = @Id;
                END
        END
        COMMIT TRANSACTION T1;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1;
                SET @Validate = 0;
                SET @Message = N'ویرایش انجام نشد';
                SET @Code = N'400';
            END
    END CATCH
    SELECT @Message AS [Message],
           @Id AS Id,
           @Validate AS Validate,
           @Code AS Code;
END