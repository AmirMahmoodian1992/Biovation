
CREATE PROCEDURE [dbo].[SelectSettings]
@Key nvarchar(100) = NULL
AS
BEGIN
    SELECT [Id]
      ,[Key]
      ,[Value]
      ,[Description]
	FROM [dbo].[Setting]
    WHERE  ([Key] = @Key
           OR ISNULL(@Key, '') = '')           
END
