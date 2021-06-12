CREATE PROCEDURE [dbo].[SelectRelayHub]
@AdminUserId INT=NULL,@Id INT= NULL, @IpAddress NVARCHAR(50) = NULL, @Port INT = NULL, @Capacity INT = NULL, @RelayHubModelId INT = NULL, @Description NVARCHAR(MAX) = NULL, @PageNumber INT=0, @PageSize INT=0
AS
DECLARE @HasPaging AS BIT;
DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (5) = N'200';
BEGIN
    SET @HasPaging = CASE WHEN @PageSize = 0
                               AND @PageNumber = 0 THEN 0 ELSE 1 END;
    IF @HasPaging = 1
        BEGIN
			SELECT [R].[Id] AS  Data_Id,
				   [R].IpAddress AS  Data_IpAddress,
				   [R].[Port] AS  Data_Port,
				   [R].[Capacity] AS  Data_Capacity ,
				   [DM].Id AS  Data_RelayHubModel_Id,
				   [DM].[Name] AS  Data_RelayHubModel_Name,
				   [DM].[ManufactureCode] AS  Data_RelayHubModel_ManufactureCode,
				   [DM].[BrandId] AS  Data_RelayHubModel_Brand_Code,
				   [DM].[GetLogMethodType] AS  Data_RelayHubModel_GetLogMethodType,
				   [DM].[Description] AS  Data_RelayHubModel_Description,
				   [DM].[DefaultPortNumber] AS  Data_RelayHubModel_DefaultPortNumber,
				   [R].[Description] AS  Data_Description,
				    (@PageNumber - 1) * @PageSize AS [from],
                     @PageNumber AS PageNumber,
                     @PageSize AS PageSize,
                     count(*) OVER () AS [Count],
                     @Message AS e_Message,
                     @Code AS e_Code,
                     @Validate AS e_Validate
			FROM [Rly].RelayHub AS R
				 INNER JOIN
				 [dbo].DeviceModel AS DM
				 ON [R].[RelayHubModelId] = [DM].Id
			WHERE (isnull(@AdminUserId, 0) = 0
                      OR EXISTS (SELECT Id
                                 FROM   [dbo].[USER]
                                 WHERE  IsMasterAdmin = 1
                                        AND ID = @AdminUserId))
					 AND ([R].Id = @Id
						  OR ISNULL(@Id,0) = 0)
                     AND ([R].IpAddress = @IpAddress
						  OR ISNULL(@IpAddress,'') = '')
					 AND ([R].[Port] = @Port
						  OR ISNULL(@Port,0) = 0)
   				     AND ([R].[Capacity] = @Capacity
						  OR ISNULL(@Capacity,0) = 0)
					 AND ([R].[RelayHubModelId] = @RelayHubModelId
						  OR ISNULL(@RelayHubModelId,0) = 0)
			ORDER BY [R].Id
            OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
		END
		ELSE
			BEGIN
				SELECT [R].[Id] AS  Data_Id,
				   [R].IpAddress AS  Data_IpAddress,
				   [R].[Port] AS  Data_Port,
				   [R].[Capacity] AS  Data_Capacity ,
				   [DM].Id AS  Data_RelayHubModel_Id,
				   [DM].[Name] AS  Data_RelayHubModel_Name,
				   [DM].[ManufactureCode] AS  Data_RelayHubModel_ManufactureCode,
				   [DM].[BrandId] AS  Data_RelayHubModel_Brand_Code,
				   [DM].[GetLogMethodType] AS  Data_RelayHubModel_GetLogMethodType,
				   [DM].[Description] AS  Data_RelayHubModel_Description,
				   [DM].[DefaultPortNumber] AS  Data_RelayHubModel_DefaultPortNumber,
				   [R].[Description] AS  Data_Description,
				   1 AS [from],
				   1 AS PageNumber,
				   count(*) OVER () AS PageSize,
				   count(*) OVER () AS [Count],
                   @Message AS e_Message,
                   @Code AS e_Code,
                   @Validate AS e_Validate
			FROM [Rly].RelayHub AS R
				 INNER JOIN
				 [dbo].DeviceModel AS DM
				 ON [R].[RelayHubModelId] = [DM].Id
			WHERE (isnull(@AdminUserId, 0) = 0
                      OR EXISTS (SELECT Id
                                 FROM   [dbo].[USER]
                                 WHERE  IsMasterAdmin = 1
                                        AND ID = @AdminUserId))
					 AND ([R].Id = @Id
						  OR ISNULL(@Id,0) = 0)
                     AND ([R].IpAddress = @IpAddress
						  OR ISNULL(@IpAddress,'') = '')
					 AND ([R].[Port] = @Port
						  OR ISNULL(@Port,0) = 0)
   				     AND ([R].[Capacity] = @Capacity
						  OR ISNULL(@Capacity,0) = 0)
					 AND ([R].[RelayHubModelId] = @RelayHubModelId
						  OR ISNULL(@RelayHubModelId,0) = 0)
			END
	 
END