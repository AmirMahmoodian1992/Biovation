CREATE PROCEDURE [dbo].[DeleteShedulingById]
@Id INT
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'حذف با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0;
    BEGIN TRY
        BEGIN TRANSACTION T1;
		BEGIN
			IF EXISTS (SELECT Id
				FROM [Rly].[Sheduling]
				WHERE Id = @Id)
				BEGIN
					DELETE [Rly].[Sheduling]
					WHERE Id = @Id
				END
		END
	 COMMIT TRANSACTION T1;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1;
                SET @Validate = 0;
                SET @Message = N'حذف انجام نشد';
            END
    END CATCH
    SELECT @Message AS [Message],
           @Id AS Id,
           @Validate AS Validate;
END