
CREATE PROCEDURE [dbo].[InsertDeviceDynamicType]
@ID INT, @Name NVARCHAR (100), @Description NVARCHAR (100)=NULL
AS
BEGIN
    IF NOT EXISTS (SELECT ID
                   FROM   [DynamicPropertyTypes]
                   WHERE  Name = @Name)
        BEGIN
            INSERT  INTO [dbo].[DynamicPropertyTypes] ([ID], [Name], [Description])
            VALUES                                   (@ID, @Name, @Description);
        END
END
GO
