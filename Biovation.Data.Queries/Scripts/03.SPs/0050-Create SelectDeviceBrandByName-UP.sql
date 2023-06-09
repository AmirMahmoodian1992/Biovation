
CREATE PROCEDURE [dbo].[SelectDeviceBrandByName]
@Name NVARCHAR (100)
AS
BEGIN
    IF EXISTS (SELECT code
               FROM   [Lookup]
               WHERE  Name = @Name)
        BEGIN
            SELECT [code],
                   [Name],
                   [Description]
            FROM   [dbo].[Lookup]
            WHERE  Name = @Name;
        END
END
GO
