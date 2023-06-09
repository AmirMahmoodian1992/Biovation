CREATE PROCEDURE [dbo].[InsertDeviceModel]
@ID INT, @Name NVARCHAR (100), @ManufactureCode INT, @BrandID INT, @GetLogMethodType INT=0, @Description NVARCHAR (100)=NULL,@DefaultPortNumber INT=NULL
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ثبت با موفقیت انجام گرفت', @Validate AS INT = 1,@Code AS NVARCHAR (15) = N'201';
    BEGIN TRY
        BEGIN TRANSACTION T1
        BEGIN
        IF NOT EXISTS (SELECT ID
                   FROM   [DeviceModel]
                   WHERE  ID = @ID
                          OR ([Name] = @Name
                              AND [BrandID] = @BrandID
                              AND [ManufactureCode] = @ManufactureCode))
        BEGIN
            INSERT  INTO [dbo].[DeviceModel] ([ID], [Name], [ManufactureCode], [BrandID], [GetLogMethodType], [Description],[DefaultPortNumber])
            VALUES                          (@ID, @Name, @ManufactureCode, @BrandID, @GetLogMethodType, @Description,@DefaultPortNumber);
        END
        END
        SET @ID = SCOPE_IDENTITY()
        COMMIT TRANSACTION T1
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1
                SET @Validate = 0
                SET @Message = N'ثبت انجام نشد'
                SET @Code = N'400';
            END
    END CATCH
    SELECT @Message AS [Message],
           @ID AS Id,
           @Validate AS Validate,
           @Code AS Code
END