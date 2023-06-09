
CREATE PROCEDURE [dbo].[SelectDynamicPropertyTypeById]
@Id INT
AS
BEGIN
    SELECT [Id],
           [Name],
           [Description]
    FROM   [dbo].[DynamicPropertyTypes]
    WHERE  Id = @Id;
END
GO
