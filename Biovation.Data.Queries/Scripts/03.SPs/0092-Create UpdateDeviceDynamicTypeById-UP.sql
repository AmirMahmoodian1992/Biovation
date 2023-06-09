
CREATE PROCEDURE [dbo].[UpdateDeviceDynamicTypeById]
@Id INT, @Name NVARCHAR (100), @Description NVARCHAR (100)=NULL
AS
BEGIN
    IF EXISTS (SELECT Id
               FROM   [DynamicPropertyTypes]
               WHERE  Id = @Id)
        BEGIN
            UPDATE [dbo].[DynamicPropertyTypes]
            SET    [Name]        = @Name,
                   [Description] = ISNULL(@Description, [Description])
            WHERE  Id = @Id;
        END
END
GO
