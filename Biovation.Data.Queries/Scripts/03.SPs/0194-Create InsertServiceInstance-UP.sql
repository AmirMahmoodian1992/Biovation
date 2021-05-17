Create PROCEDURE [dbo].[InsertServiceInstance]
@Id NVARCHAR (MAX),@Name NVARCHAR (MAX) = NULL, @Version NVARCHAR (MAX) = NULL, @Ip NVARCHAR (MAX) = NULL, @Port INT = NULL,@Description NVARCHAR (MAx)=NULL
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0;
    BEGIN TRY
        BEGIN TRANSACTION T1;       
		IF NOT EXISTS (SELECT *
                           FROM   [dbo].[ServiceInstance] 
                           WHERE  Id=@Id )
            BEGIN
              INSERT  INTO [dbo].[ServiceInstance]  ([Id], [Name], [Version], [Ip], [Port], [Description])
	            VALUES                          (@Id, @Name, @Version, @Ip, @Port, @Description);
                SET @Code = SCOPE_IDENTITY();
            END
        ELSE
            BEGIN
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