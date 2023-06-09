
CREATE PROCEDURE [dbo].[SelectDeviceDynamicPropertiesTypeByModelId]
@ModelId INT
AS
BEGIN
    SELECT [Id],
           [Name],
           [TypeId],
           [ModelId],
           [Description]
    FROM   [dbo].[DeviceDynamicPropertyName]
    WHERE  ModelId = @ModelId;
END
GO
