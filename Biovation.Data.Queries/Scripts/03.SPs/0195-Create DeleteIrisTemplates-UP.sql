Create PROCEDURE [dbo].[DeleteIrisTemplates]
@UserId BIGINT, @Index INT=NULL
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'حذف با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (15) = N'200'
    BEGIN TRY
        BEGIN TRANSACTION T1;
        DELETE [dbo].[IrisTemplate]
        WHERE  UserId = @UserId	
                AND ([index] = @index
                   OR ISNULL(@index, 0) = 0)
        COMMIT TRANSACTION T1;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1;
                SET @Validate = 0;
                SET @Message = N'حذف انجام نشد';
				SET @Code=400
            END
    END CATCH
    SELECT @Message AS [Message],
           @UserId AS Id,
		   @Code AS Code,
           @Validate AS Validate;
END