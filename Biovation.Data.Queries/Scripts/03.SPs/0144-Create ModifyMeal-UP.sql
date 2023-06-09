
CREATE PROCEDURE [dbo].[ModifyMeal]
@Id int,
@Name nvarchar(150), 
@Description nvarchar(500)

AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0
    BEGIN TRY
        BEGIN TRANSACTION T1
        BEGIN
            IF NOT EXISTS (SELECT Id
                           FROM   [Rst].[Meal]
                           WHERE  Id = @Id)
                BEGIN
                    INSERT  INTO [Rst].[Meal] ([Name], [Description]) VALUES (@Name, @Description)
					SELECT @Id = SCOPE_IDENTITY()
                END
            ELSE
			  BEGIN
                UPDATE [Rst].[Meal]
                SET    [Name]  = @Name,
                       [Description]    = @Description
                WHERE  Id = @Id
				SET @Message = N'ویرایش با موفقیت انجام گرفت'
			END
        END
        
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
           @Id AS Id,
           @Validate AS Validate
END