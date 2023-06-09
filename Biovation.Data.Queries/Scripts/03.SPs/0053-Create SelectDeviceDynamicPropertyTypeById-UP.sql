
CREATE PROCEDURE [dbo].[SelectDeviceDynamicPropertyTypeById]
@Id INT
AS
BEGIN
    SELECT [Id],
           [Name],
           [TypeId],
           [ModelId],
           [Description]
    FROM   [dbo].[DeviceDynamicPropertyName]
    WHERE  Id = @Id;
END
GO
