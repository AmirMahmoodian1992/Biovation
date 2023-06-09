
CREATE PROCEDURE [dbo].[UpdateDeviceModelNameById]
@Id INT, @Name NVARCHAR (100), @Description NVARCHAR (100)=NULL
AS
BEGIN
    IF EXISTS (SELECT Id
               FROM   [DeviceModel]
               WHERE  Id = @Id)
        BEGIN
            UPDATE [dbo].[DeviceModel]
            SET    [Name]        = @Name,
                   [Description] = @Description
            WHERE  Id = @Id;
        END
END
GO
