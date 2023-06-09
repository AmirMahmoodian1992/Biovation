
CREATE PROCEDURE [dbo].[ModifyFingerTemplate]
@Id INT,
@UserId bigint, 
@TemplateIndex INT, @FingerIndexCode NVARCHAR(50),
@Template VARBINARY (MAX)=NULL, @Index INT, @Duress BIT, @CheckSum INT,
@SecurityLevel INT, @EnrollQuality INT, @Size INT,
@FingerTemplateType NVARCHAR(50),
@CreateBy BIGINT = 0,
@UpdateBy BIGINT = 0
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (15) = N'201';
    BEGIN TRY
        BEGIN TRANSACTION T1;
        BEGIN
            IF @Id = 0
                INSERT  INTO [dbo].[FingerTemplate] 
				([UserId], [TemplateIndex], [FingerIndex],
				[Template], [Index], [Duress], [CheckSum],
				[SecurityLevel], [EnrollQuality], [Size],
				[FingerTemplateType],CreateAt,CreateBy)
                VALUES                  
				(@UserId, @TemplateIndex, @FingerIndexCode,
				@Template, @Index, @Duress, @CheckSum,
				@SecurityLevel, @EnrollQuality, @Size, 
				@FingerTemplateType,GetDate(),@CreateBy);
            ELSE
                UPDATE [dbo].[FingerTemplate]
                SET    [UserId]        = @UserId,
                       [TemplateIndex] = @TemplateIndex,
                       [FingerIndex]   = @FingerIndexCode,
                       [Template]      = @Template,
                       [Index]         = @Index,
                       [Duress]        = @Duress,
                       [CheckSum]      = @CheckSum,
                       [SecurityLevel] = @SecurityLevel,
                       [EnrollQuality] = @EnrollQuality,
                       [Size]          = @Size,
					   [FingerTemplateType] = @FingerTemplateType,
					   [UpdateAt] = GETDATE(),
					   [UpdateBy] = @UpdateBy
                WHERE  Id = @Id;
        END
        SET @Code = SCOPE_IDENTITY();
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
           @Code AS Code
END
GO
