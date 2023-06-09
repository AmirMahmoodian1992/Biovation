
CREATE PROCEDURE [dbo].[UpdateDeviceModelById]
@Id INT, @Name NVARCHAR (100), @BrandId INT, @Description NVARCHAR (100)=NULL,@DefaultPortNumber INT
AS
BEGIN
    IF EXISTS (SELECT Id
               FROM   [DeviceModel]
               WHERE  Id = @Id)
        BEGIN
            UPDATE [dbo].[DeviceModel]
            SET    [Name]        = @Name,
                   [BrandId]     = @BrandId,
                   [Description] = @Description,
				   [DefaultPortNumber] = @DefaultPortNumber
            WHERE  Id = @Id;
        END
END
GO

