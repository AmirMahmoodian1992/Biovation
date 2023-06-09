Create PROCEDURE [dbo].[InsertAccessGroup]
@Id INT, @Name NVARCHAR (500), @TimeZoneId INT, @Description NVARCHAR (1000)=NULL
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0;
    BEGIN TRY
        BEGIN TRANSACTION T1;       
		IF NOT EXISTS (SELECT *
                           FROM   [dbo].[AccessGroup] 
                           WHERE  Id=0 or Id=@Id )
            BEGIN
                INSERT  INTO [dbo].[AccessGroup] ([Name], [TimeZoneId], [Description])
                VALUES                          (@Name, @TimeZoneId, @Description);
                SET @Code = SCOPE_IDENTITY();
            END
        ELSE
            BEGIN
                SET @Message = N'شناسه گروه دسترسی تکراری می باشد';
                SET @Validate = 0;              
                SET @Code = @Id;
                SET @Message = N'ایجاد با خطا مواجه شد';
            END
        COMMIT TRANSACTION T1;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1;
                SET @Validate = 0;
                SET @Message = N'ایجاد انجام نشد';
            END
    END CATCH
    SELECT @Message AS [Message],
           @Code AS Id,
           @Validate AS Validate;
END