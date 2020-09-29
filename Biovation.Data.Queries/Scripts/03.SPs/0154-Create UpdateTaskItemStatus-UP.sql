
CREATE PROCEDURE [dbo].[UpdateTaskItemStatus]
@Id INT,
@statusCode NVARCHAR (50),
@CurrentIndex INT=null,
@TotalCount INT=null,
@FinishedAt DATETIMEOFFSET(7)=null,
@ExecutionAt DATETIMEOFFSET(7)=null,
@result NVARCHAR (MAX)=NULL
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ویرایش با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (5) = N'201';
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
						   [ExecutionAt] = IsNull(@ExecutionAt,ExecutionAt),
						   [FinishedAt] =IsNull( @FinishedAt,FinishedAt)						   
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