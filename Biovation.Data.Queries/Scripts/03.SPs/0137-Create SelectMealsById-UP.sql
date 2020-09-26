
CREATE PROCEDURE [dbo].[SelectMealsById]
@mealId int = 0
AS
BEGIN
    SELECT [Id]
      ,[Name]
      ,[Description]
  FROM [Rst].[Meal]
    WHERE  [Id] = @mealId
           OR ISNULL(@mealId, 0) = 0
           
END
