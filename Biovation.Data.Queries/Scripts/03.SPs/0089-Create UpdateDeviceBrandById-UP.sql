
CREATE PROCEDURE [dbo].[UpdateDeviceBrandById]
@Id INT, @Name NVARCHAR (100), @Description NVARCHAR (100)=NULL
AS
BEGIN
    IF EXISTS (SELECT Id
               FROM   [DeviceBrand]
               WHERE  Id = @Id)
        BEGIN
            UPDATE [dbo].[DeviceBrand]
            SET    [Name]        = @Name,
                   [Description] = @Description
            WHERE  Id = @Id;
        END
END
GO
