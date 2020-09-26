
CREATE PROCEDURE [dbo].[SelectDeviceBrands]
AS
BEGIN
    SELECT DevBrand.[code],
           DevBrand.[Name],
           DevBrand.[Description]
          
    FROM   [dbo].[Lookup] AS DevBrand
           
           where DevBrand.LookupCategoryId=6;
END
GO
