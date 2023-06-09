
CREATE PROCEDURE [dbo].[ModifyIrisTemplate]
@Id int,
@UserId bigint,
@Template VARBINARY (MAX)=NULL,
@Index INT,
@CheckSum INT, 
@SecurityLevel INT,
@EnrollQuality INT,
@Size INT, 
@IrisTemplateType INT
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0
    BEGIN TRY
        BEGIN TRANSACTION T1
        BEGIN
            IF @Id = 0
                INSERT  INTO [dbo].[IrisTemplate] 
				([UserId],
				[Template], 
				[Index],
				[CheckSum], 
				[SecurityLevel],
				[EnrollQuality],
				[Size], 
				[IrisTemplateType])
                VALUES          
				(@UserId, 
				 @Template, 
				 @Index,
				 @CheckSum,
				 @SecurityLevel, 
				 @EnrollQuality,
				 @Size, 
				 @IrisTemplateType)
            ELSE
                UPDATE [dbo].[IrisTemplate]
                SET    [UserId]        = @UserId,
                       [Template]      = @Template,
                       [Index]         = @Index,
                       [CheckSum]      = @CheckSum,
                       [SecurityLevel] = @SecurityLevel,
                       [EnrollQuality] = @EnrollQuality,
                       [Size]          = @Size,
					   [IrisTemplateType] = @IrisTemplateType
                WHERE  Id = @Id
        END
        SET @Code = SCOPE_IDENTITY()
        COMMIT TRANSACTION T1
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1
                SET @Validate = 0
                SET @Message = N'ایجاد انجام نشد'
            END
    END CATCH
    SELECT @Message AS [Message],
           @Code AS Id,
           @Validate AS Validate
END
GO
