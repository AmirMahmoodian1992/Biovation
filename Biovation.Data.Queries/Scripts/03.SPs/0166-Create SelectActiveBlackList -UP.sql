Create PROCEDURE [dbo].[SelectActiveBlackList]

@Id int=0,
@UserId int=0,
@DeviceId int=0,
@Today datetime=0,
@Isdeleted bit=0

AS
BEGIN
    SELECT [BL].[Id]
	  ,[User].[Id] AS User_Id
      ,[Device].[Id] AS Device_DeviceId
	  ,[Device].[Code] AS Device_Code
	  ,[DevModel].[BrandId] AS Device_Brand_Code
	  ,[DevBrand].[Name] AS Device_Brand_Name
      ,[BL].[IsDeleted]
      ,[BL].[Description]
	  ,[BL].[StartDate]
      ,[BL].[EndDate]


  FROM [dbo].[BlackList] AS [BL]
     INNER JOIN [dbo].[User] AS [User] ON [BL].[UserId] = [User].[Id]
     INNER JOIN [dbo].[Device] AS [Device] ON [BL].[DeviceId] = [Device].[Id]
     INNER JOIN [dbo].[DeviceModel] AS [DevModel] ON [Device].[DeviceModelId] = [DevModel].[Id]
	 INNER JOIN [dbo].[lookup] AS [DevBrand] ON [DevModel].[brandId] = [DevBrand].[code]
     
   
    WHERE   ([BL].[Id] = @Id OR ISNULL(@Id, 0) = 0) 
		AND ([BL].[UserId] = @UserId OR ISNULL(@UserId, 0) = 0)
		AND [BL].[IsDeleted] = @Isdeleted 
		AND ([BL].[DeviceId] = @DeviceId OR ISNULL(@DeviceId, 0) = 0)
		AND (([BL].[StartDate] >= @Today AND  @Today <=[BL].[EndDate] )  OR ISNULL(@today, 0) = 0 OR ISNULL(@today, 0) = 0 )

END