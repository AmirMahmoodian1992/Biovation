
CREATE PROCEDURE [dbo].[SelectDynamicPropertyTypeByName]
@Name VARCHAR (100)
AS
BEGIN
    SELECT [Id],
           [Name],
           [Description]
    FROM   [dbo].[DynamicPropertyTypes]
    WHERE  Name = @Name;
END
GO
