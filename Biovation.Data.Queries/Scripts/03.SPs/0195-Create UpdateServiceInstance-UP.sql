Create PROCEDURE [dbo].[UpdateServiceInstance]
@Id NVARCHAR (MAX),@Name NVARCHAR (MAX) = NULL, @Version NVARCHAR (MAX) = NULL, @Ip NVARCHAR (MAX) = NULL, @Port INT = NULL, @Description NVARCHAR (MAX)=NULL, @Health  BIT
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'به روز رسانی با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0;
    BEGIN TRY
        BEGIN TRANSACTION T1;       
		IF EXISTS (SELECT *
                           FROM   [dbo].[ServiceInstance] 
                           WHERE  Id=@Id )
            BEGIN
              UPDATE  [dbo].[ServiceInstance]  
              SET   [Name] = @Name,
                    [Version] = @Version,
                    [Ip] = @Ip,
                    [Port] = @Port,
                    [Description] = @Description
              WHERE [Id] = @Id;
	                                 
                SET @Code = SCOPE_IDENTITY();
            END
        ELSE
            BEGIN
                SET @Validate = 0;              
                SET @Code = @Id;
                SET @Message = N'به روز رسانی با خطا مواجه شد';
            END
        COMMIT TRANSACTION T1;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1;
                SET @Validate = 0;
                SET @Message = N'به روز رسانی انجام نشد';
            END
    END CATCH
    SELECT @Message AS [Message],
           @Code AS Id,
           @Validate AS Validate;
END