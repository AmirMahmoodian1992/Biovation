
CREATE PROCEDURE [dbo].[SelectMealTimingsByMealId]
@mealId int = 0
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

	  ,[Dev].[Id] AS Devices_DeviceId
	  ,[Dev].[Code] AS Devices_Code
	  ,[Dev].[DeviceModelId] AS Devices_DeviceModelId
	  ,[Dev].[Name] AS Devices_Name
	  ,[Dev].[Active] AS Devices_Active
	  ,[Dev].[IPAddress] AS Devices_IpAddress
	  ,[Dev].[Port] AS Devices_Port
	  ,[Dev].[DeviceTypeId] AS Devices_DeviceTypeId

  FROM [Rst].[MealTiming] AS [MT]
	LEFT JOIN [Rst].[Meal] AS [Meal] ON [Mt].[MealId] = [Meal].[Id]
	LEFT JOIN [dbo].[Device] AS [Dev] ON [Mt].[DeviceId] = [Dev].[Id]
    WHERE  [MT].[MealId] = @mealId
           
END
