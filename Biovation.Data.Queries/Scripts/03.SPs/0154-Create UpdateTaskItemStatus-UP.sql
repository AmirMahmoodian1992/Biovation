
CREATE PROCEDURE [dbo].[UpdateTaskItemStatus]
@Id int,
@statusCode nvarchar(50),
@result nvarchar(MAX) = NULL

AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ویرایش با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 201
    BEGIN TRY
        BEGIN TRANSACTION T1
        BEGIN
            IF EXISTS (SELECT Id
                           FROM   [dbo].[TaskItem]
                           WHERE  Id = @Id)
                BEGIN
                UPDATE [dbo].[TaskItem]
                SET    [StatusCode]  = @statusCode,
					   [Result] = @result
                WHERE  Id = @Id
			END
        END
        
        COMMIT TRANSACTION T1
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1
                SET @Validate = 0
                SET @Message = N'ویرایش انجام نشد'
                SET @Code  = 400
            END
    END CATCH
    SELECT @Message AS [Message],
           @Id AS Id,
           @Validate AS Validate,
           @Code AS Code
END