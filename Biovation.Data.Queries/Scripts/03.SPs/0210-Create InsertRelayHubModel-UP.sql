CREATE PROCEDURE [dbo].[InsertRelayHubModel]
@Name nvarchar(MAX), @ManufactureCode int, @BrandId int, @DefaultPortNumber int, @DefaultCapacity int,@Description nvarchar(MAX)
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0;
    BEGIN TRY
        BEGIN TRANSACTION T1;
		BEGIN
			IF NOT EXISTS (SELECT ID 
				FROM [Rly].RelayHubModel
				WHERE [ManufactureCode] = @ManufactureCode
					AND [BrandId] = @BrandId
					AND [Name] = @Name
                    AND [DefaultCapacity] = @DefaultCapacity)
				BEGIN
					INSERT INTO [Rly].RelayHubModel ([Name], [ManufactureCode], [BrandId], [DefaultPortNumber], [DefaultCapacity], [Description])
					VALUES (@Name, @ManufactureCode, @BrandId, @DefaultPortNumber, @DefaultCapacity, @Description)
				END
		END
        SET @Code = SCOPE_IDENTITY();
        COMMIT TRANSACTION T1;
    END TRY	
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1;
                SET @Validate = 0;
                SET @Message = N'ثبت انجام نشد';
            END
    END CATCH
    SELECT @Message AS [Message],
           @Code AS Id,
           @Validate AS Validate;
END
