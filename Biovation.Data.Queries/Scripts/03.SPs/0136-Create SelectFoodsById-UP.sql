
CREATE PROCEDURE [dbo].[SelectFoodsById]
@foodId int = 0
AS
BEGIN
    SELECT [Id]
      ,[Name]
      ,[Description]
  FROM [Rst].[Food]
    WHERE  [Id] = @foodId
           OR ISNULL(@foodId, 0) = 0
           
END
