
CREATE PROCEDURE [dbo].[ModifyAccessGroup]
@Id INT, @Name NVARCHAR (500), @TimeZoneId INT, @Description NVARCHAR (1000)=NULL, @AdminUserId INT = NULL
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1,@Code AS NVARCHAR (15) = N'201';
    BEGIN TRY
        BEGIN TRANSACTION T1
        IF (@ID = 0)
            BEGIN
                INSERT  INTO [dbo].[AccessGroup] ([Name], [TimeZoneId], [Description])--, [AdminUserId])
                VALUES                          (@Name, @TimeZoneId, @Description)--, @AdminUserId)
                SET @Code = SCOPE_IDENTITY()
            END
        ELSE
            BEGIN
                UPDATE [dbo].[AccessGroup]
                SET    [TimeZoneId]  = @TimeZoneId,
                       [Description] = @Description,
                       [Name]        = @Name
                       --[AdminUserId] = @AdminUserId
                WHERE  Id = @Id
				                SET @Code = @Id
                              
                                
                SET @Message = N'ویرایش با موفقیت انجام شد'
            END
        COMMIT TRANSACTION T1
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1
                SET @Validate = 0
                SET @Message = N'ایجاد انجام نشد'
                SET @Code  = N'400';
            END
    END CATCH
    SELECT @Message AS [Message],
           @Code AS Id,
           @Validate AS Validate,
           @Code AS Code
END
GO
