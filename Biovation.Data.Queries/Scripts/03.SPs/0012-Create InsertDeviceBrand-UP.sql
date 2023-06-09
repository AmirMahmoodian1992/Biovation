
CREATE PROCEDURE [dbo].[InsertDeviceBrand]
@ID INT, @Name NVARCHAR (100), @Description NVARCHAR (100)=NULL
AS
BEGIN
    IF NOT EXISTS (SELECT ID
                   FROM   [DeviceBrand]
                   WHERE  ID = @ID
                          OR Name = @Name)
        BEGIN
            INSERT  INTO [dbo].[DeviceBrand] ([ID], [Name], [Description])
            VALUES                          (@ID, @Name, @Description);
        END
END
GO
