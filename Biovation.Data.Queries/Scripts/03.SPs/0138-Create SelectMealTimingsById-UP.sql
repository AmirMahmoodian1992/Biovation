
CREATE PROCEDURE [dbo].[SelectMealTimingsById]
@mealTimingId int = 0
AS
BEGIN
    SELECT [MT].[Id]
      ,[MT].[DeviceId]
      ,[MT].[StartDate]
      ,[MT].[EndDate]
      ,[MT].[StartTimeInMinutes]
      ,[MT].[EndTimeInMinutes]

	  ,[Meal].[Id] AS Meal_Id
      ,[Meal].[Name] AS Meal_Name
      ,[Meal].[Description] AS Meal_Description

	  ,[Dev].[Id] AS Device_DeviceId
	  ,[Dev].[Code] AS Device_Code
	  ,[Dev].[DeviceModelId] AS Device_DeviceModelId
	  ,[Dev].[Name] AS Device_Name
	  ,[Dev].[Active] AS Device_Active
	  ,[Dev].[IPAddress] AS Device_IpAddress
	  ,[Dev].[Port] AS Device_Port
	  ,[Dev].[DeviceTypeId] AS Device_DeviceTypeId

  FROM [Rst].[MealTiming] AS [MT]
	INNER JOIN [Rst].[Meal] AS [Meal] ON [Mt].[MealId] = [Meal].[Id]
	INNER JOIN [dbo].[Device] AS [Dev] ON [Mt].[DeviceId] = [Dev].[Id]
    WHERE  [MT].[Id] = @mealTimingId
           OR ISNULL(@mealTimingId, 0) = 0
           
END
