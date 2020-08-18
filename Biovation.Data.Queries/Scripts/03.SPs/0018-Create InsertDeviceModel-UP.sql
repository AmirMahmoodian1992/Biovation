
CREATE PROCEDURE [dbo].[InsertDeviceModel]
@ID INT, @Name NVARCHAR (100), @ManufactureCode INT, @BrandID INT, @GetLogMethodType INT=0, @Description NVARCHAR (100)=NULL,@DefaultPortNumber INT=NULL
AS
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