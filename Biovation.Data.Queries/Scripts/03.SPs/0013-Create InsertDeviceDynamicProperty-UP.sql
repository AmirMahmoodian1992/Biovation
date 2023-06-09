
CREATE PROCEDURE [dbo].[InsertDeviceDynamicProperty]
@Name NVARCHAR (100), @TypeId INT, @ModelId INT, @Description NVARCHAR (100)=NULL
AS
BEGIN
    IF NOT EXISTS (SELECT ID
                   FROM   [DeviceDynamicPropertyName]
                   WHERE  Name = @Name)
        BEGIN
            INSERT  INTO [dbo].[DeviceDynamicPropertyName] ([Name], [TypeId], [ModelId], [Description])
            VALUES                                        (@Name, @TypeId, @ModelId, @Description);
        END
END
GO
