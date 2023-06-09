
CREATE PROCEDURE [dbo].[UpdateDeviceDynamicPropertyById]
@Id INT, @Name NVARCHAR (100), @TypeId INT, @ModelId INT, @Description NVARCHAR (100)=NULL
AS
BEGIN
    IF NOT EXISTS (SELECT Id
                   FROM   [DeviceDynamicPropertyName]
                   WHERE  Id = @Id)
        BEGIN
            UPDATE [dbo].[DeviceDynamicPropertyName]
            SET    [Name]        = @Name,
                   [TypeId]      = @TypeId,
                   [ModelId]     = @ModelId,
                   [Description] = ISNULL(@Description, [Description])
            WHERE  Id = @Id;
        END
END
GO
