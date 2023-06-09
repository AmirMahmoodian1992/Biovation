CREATE PROCEDURE [dbo].[SelectBlackList] 
@Id INT=0, @UserId INT=0, @DeviceId INT=0, @StartDate DATETIME=0, @EndDate DATETIME=0, @Isdeleted BIT=0,@PageNumber AS INT=0, @PageSize AS INT =0
AS
          DECLARE  @HasPaging   BIT;
		  DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (15) = N'200';
   
BEGIN

 SET @HasPaging = CASE
                                 WHEN @PageSize =0
                                      AND @PageNumber =0 THEN
                                     0
                                 ELSE
                                     1
                             END;
 IF @HasPaging = 1
BEGIN
SELECT [BL].[Id] AS Data_Id,
           [User].[Id] AS Data_User_Id,
           [Device].[Id] AS Data_Device_DeviceId,
           [Device].[Code] AS Data_Device_Code,
           [DevModel].[BrandId] AS Data_Device_Brand_Code,
           [DevBrand].[Name] AS Data_Device_Brand_Name,
           [BL].[IsDeleted] AS Data_IsDeleted,
           [BL].[Description] AS Data_Description,
           [BL].[StartDate] AS Data_StartDate,
           [BL].[EndDate] AS Data_EndDate,
		   (@PageNumber-1)*@PageSize  AS [from],
		   @PageNumber AS PageNumber,
		   @PageSize As PageSize,
		   count(*) OVER() AS [Count],
		   @Message AS e_Message,
		   @Code  AS e_Code,
		   @Validate  AS e_Validate
    FROM   [dbo].[BlackList] AS [BL]
           INNER JOIN
           [dbo].[User] AS [User]
           ON [BL].[UserId] = [User].[Id]
           INNER JOIN
           [dbo].[Device] AS [Device]
           ON [BL].[DeviceId] = [Device].[Id]
           INNER JOIN
           [dbo].[DeviceModel] AS [DevModel]
           ON [Device].[DeviceModelId] = [DevModel].[Id]
           INNER JOIN
           [dbo].[lookup] AS [DevBrand]
           ON [DevModel].[brandId] = [DevBrand].[code]
    WHERE  ([BL].[Id] = @Id
            OR ISNULL(@Id, 0) = 0)
           AND ([BL].[UserId] = @UserId
                OR ISNULL(@UserId, 0) = 0)
           AND [BL].[IsDeleted] = @Isdeleted
           AND ([BL].[DeviceId] = @DeviceId
                OR ISNULL(@DeviceId, 0) = 0)
           AND (([BL].[StartDate] >= @StartDate
                 AND [BL].[EndDate] <= @EndDate)
                OR ISNULL(@StartDate, 0) = 0
                OR ISNULL(@EndDate, 0) = 0)
           ORDER BY [BL].[Id]
           OFFSET (@PageNumber-1)*@PageSize ROWS
           FETCH NEXT @PageSize ROWS ONLY	
END
ELSE
BEGIN
SELECT [BL].[Id] AS Data_Id,
           [User].[Id] AS Data_User_Id,
           [Device].[Id] AS Data_Device_DeviceId,
           [Device].[Code] AS Data_Device_Code,
           [DevModel].[BrandId] AS Data_Device_Brand_Code,
           [DevBrand].[Name] AS Data_Device_Brand_Name,
           [BL].[IsDeleted] AS Data_IsDeleted,
           [BL].[Description] AS Data_Description,
           [BL].[StartDate] AS Data_StartDate,
           [BL].[EndDate] AS Data_EndDate,
		   1 AS [from],
		   1 AS PageNumber,
		   count(*) OVER() As PageSize,
		   count(*) OVER() AS [Count],
		   @Message AS e_Message,
		   @Code  AS e_Code,
		   @Validate  AS e_Validate
    FROM   [dbo].[BlackList] AS [BL]
           INNER JOIN
           [dbo].[User] AS [User]
           ON [BL].[UserId] = [User].[Id]
           INNER JOIN
           [dbo].[Device] AS [Device]
           ON [BL].[DeviceId] = [Device].[Id]
           INNER JOIN
           [dbo].[DeviceModel] AS [DevModel]
           ON [Device].[DeviceModelId] = [DevModel].[Id]
           INNER JOIN
           [dbo].[lookup] AS [DevBrand]
           ON [DevModel].[brandId] = [DevBrand].[code]
    WHERE  ([BL].[Id] = @Id
            OR ISNULL(@Id, 0) = 0)
           AND ([BL].[UserId] = @UserId
                OR ISNULL(@UserId, 0) = 0)
           AND [BL].[IsDeleted] = @Isdeleted
           AND ([BL].[DeviceId] = @DeviceId
                OR ISNULL(@DeviceId, 0) = 0)
           AND (([BL].[StartDate] >= @StartDate
                 AND [BL].[EndDate] <= @EndDate)
                OR ISNULL(@StartDate, 0) = 0
                OR ISNULL(@EndDate, 0) = 0);
END
END