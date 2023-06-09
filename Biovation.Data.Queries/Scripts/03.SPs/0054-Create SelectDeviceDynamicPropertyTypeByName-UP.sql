
CREATE PROCEDURE [dbo].[SelectDeviceDynamicPropertyTypeByName]
@Name VARCHAR (100)
AS
BEGIN
    SELECT [Id],
           [Name],
           [TypeId],
           [ModelId],
           [Description]
    FROM   [dbo].[DeviceDynamicPropertyName]
    WHERE  Name = @Name;
END
GO
