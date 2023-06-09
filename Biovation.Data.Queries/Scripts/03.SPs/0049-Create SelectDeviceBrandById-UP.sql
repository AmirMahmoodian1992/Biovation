
CREATE PROCEDURE [dbo].[SelectDeviceBrandById]
@Id INT
AS
BEGIN
       IF EXISTS (SELECT code
               FROM   [lookup]
               WHERE  code = @Id)
        BEGIN
            SELECT [DevBrand].[code],
                   [DevBrand].[Name],
                   [DevBrand].[Description]
             
            FROM   [dbo].[lookup] AS DevBrand
               
            WHERE  DevBrand.[code] = @Id;
        END
END
GO
