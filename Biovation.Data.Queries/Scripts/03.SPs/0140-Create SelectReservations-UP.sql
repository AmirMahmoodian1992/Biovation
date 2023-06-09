
CREATE PROCEDURE [dbo].[SelectReservations]
@reservationId int = 0,
@deviceId int = 0
AS
BEGIN
    SELECT [Res].[Id]
      ,[Res].[Count]
      ,[Res].[ReserveTime]
      ,[Res].[TimeStamp]

	  ,[Food].[Id] AS Food_Id
	  ,[Food].[Name] AS Food_Name
      ,[Food].[Description] AS Food_Description

	  ,[Meal].[Id] AS Meal_Id
	  ,[Meal].[Name] AS Meal_Name
      ,[Meal].[Description] AS Meal_Description

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

  FROM [RST].[Reservation] AS [Res]
	FULL JOIN [Rst].[Food] AS [Food] ON [Res].[FoodId] = [Food].[Id]
	FULL JOIN [Rst].[Meal] AS [Meal] ON [Res].[MealId] = [Meal].[Id]
	FULL JOIN [Rst].[Restaurant] AS [Rest] ON [Res].[RestaurantId] = [Rest].[Id]
	--FULL JOIN [Rst].[ReservationStatus] AS [RS] ON [Res].[StatusId] = [Rs].[Id]
	FULL JOIN [dbo].[User] AS [User] ON [Res].[UserId] = [User].[Id]
	FULL JOIN [dbo].[Device] AS [Device] On [Res].[RestaurantId] = [Device].[RestaurantId]
    WHERE  ([Res].[Id] = @reservationId
           OR ISNULL(@reservationId, 0) = 0)
		AND ([Device].[Id] = @deviceId
           OR ISNULL(@deviceId, 0) = 0)
           
END
