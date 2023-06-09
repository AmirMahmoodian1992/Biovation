
CREATE PROCEDURE [dbo].[SelectRestaurantsById]
@restaurantId int = 0
AS
BEGIN
    SELECT [Res].[Id]
		  ,[Res].[Name]
		  ,[Res].[Description]

		  ,[Dev].[Id] AS Devices_DeviceId
		  ,[Dev].[Code] AS Devices_Code
		  ,[Dev].[DeviceModelId] AS Devices_DeviceModelId
		  ,[Dev].[Name] AS Devices_Name
		  ,[Dev].[Active] AS Devices_Active
		  ,[Dev].[IPAddress] AS Devices_IpAddress
		  ,[Dev].[Port] AS Devices_Port
		  ,[Dev].[DeviceTypeId] AS Devices_DeviceTypeId

  FROM [Rst].[Restaurant] AS [Res]
		LEFT JOIN [dbo].[Device] AS [Dev]
		ON [Dev].[RestaurantId] = [Res].[Id]
    WHERE  [Res].[Id] = @restaurantId
           OR ISNULL(@restaurantId, 0) = 0
           
END
