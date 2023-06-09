
CREATE PROCEDURE [dbo].[SelectServeLogsById]
@serveLogId int = 0
AS
BEGIN
    SELECT [Srv].[Id]
      ,[Srv].[DeviceId]
      ,[Srv].[TimeStamp]
      ,[Srv].[IsSynced]

	  ,[Food].[Id] AS Food_Id
	  ,[Food].[Name] AS Food_Name
      ,[Food].[Description] AS Food_Description

	  ,[Meal].[Id] AS Meal_Id
	  ,[Meal].[Name] AS Meal_Name
      ,[Meal].[Description] AS Meal_Description
	  
	  ,[Stat].[Id] AS Status_Id
      ,[Stat].[Name] AS Status_Name
      ,[Stat].[Description] AS Status_Description

	  ,[User].[Id] AS [User_Id]
      ,[User].[FirstName] AS User_FirstName
      ,[User].[SurName] AS User_SurName
      ,[User].[UserName] AS User_UserName
      ,[User].[StartDate] AS User_StartDate
      ,[User].[EndDate] AS User_EndDate
      ,[User].[RegisterDate] AS User_RegisterDate
      ,[User].[IsActive] AS User_IsActive

	  ,[Rest].[Id] AS Restaurant_Id
      ,[Rest].[Name] AS Restaurant_Name
      ,[Rest].[Description] AS Restaurant_Description

	  ,[Device].[Id] AS Restaurant_Devices_DeviceId
      ,[Device].[Code] AS Restaurant_Devices_Code
      ,[Device].[DeviceModelId] AS Restaurant_Devices_DeviceModelId
      ,[Device].[Name] AS Restaurant_Devices_Name
      ,[Device].[Active] AS Restaurant_Devices_Active
      ,[Device].[IPAddress] AS Restaurant_Devices_IpAddress
      ,[Device].[Port] AS Restaurant_Devices_Port
      ,[Device].[DeviceTypeId] AS Restaurant_Devices_DeviceTypeId

  FROM [RST].[ServeLog] AS [Srv]
	LEFT JOIN [Rst].[Food] AS [Food] ON [Srv].[FoodId] = [Food].[Id]
	LEFT JOIN [Rst].[Meal] AS [Meal] ON [Srv].[MealId] = [Meal].[Id]
	LEFT JOIN [dbo].[User] AS [User] ON [Srv].[UserId] = [User].[Id]
	LEFT JOIN [dbo].[Device] AS [Device] On [Srv].[DeviceId] = [Device].[Id]
	LEFT JOIN [Rst].[Restaurant] AS [Rest] ON [Device].[RestaurantId] = [Rest].[Id]
	LEFT JOIN [Rst].[ServeLogStatus] AS [Stat] ON [Srv].[StatusId] = [Stat].[Id]
    WHERE  [Srv].[Id] = @serveLogId
           OR ISNULL(@serveLogId, 0) = 0
           
END
