create PROCEDURE [dbo].[SelectRelayHubModel]
@Id INT = NULL,@Name nvarchar(MAX)= NULL, @ManufactureCode int= NULL, @BrandId int= NULL, @DefaultPortNumber int= NULL, @DefaultCapacity int= NULL,@Description nvarchar(MAX)= NULL, @PageNumber INT=0, @PageSize INT=0
AS
DECLARE @HasPaging AS BIT;
DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (5) = N'200';
BEGIN
    SET @HasPaging = CASE WHEN @PageSize = 0
                               AND @PageNumber = 0 THEN 0 ELSE 1 END;
    IF @HasPaging = 1
        BEGIN
			SELECT	[RHM].[Id] AS Data_Id,
					[RHM].[Name] AS Data_Name,
					[RHM].[ManufactureCode] AS Data_ManufactureCode,
					[RHM].[DefaultPortNumber] AS Data_DefaultPortNumber,
					[RHM].[DefaultCapacity] AS Data_DefaultCapacity,
					[RHM].[Description] AS Data_Description,
				    [RHBrand].[code] AS  Data_Brand_code,
				    [RHBrand].[Name] AS  Data_Brand_Name,
				    [RHBrand].[OrderIndex] AS  Data_Brand_OrderIndex,
				    [RHBrand].[Description] AS  Data_Brand_Description,
				    (@PageNumber - 1) * @PageSize AS [from],
                     @PageNumber AS PageNumber,
                     @PageSize AS PageSize,
                     count(*) OVER () AS [Count],
                     @Message AS e_Message,
                     @Code AS e_Code,
                     @Validate AS e_Validate
			FROM [Rly].[RelayHubModel] AS RHM
			INNER JOIN
            [dbo].[lookup] AS RHBrand
            ON RHM.BrandId = RHBrand.code
			WHERE   ([RHM].[Id] = @Id
						  OR ISNULL(@Id,'') = '')
					 AND ([RHM].Name = @Name
						  OR ISNULL(@Name,'') = '')
					 AND ([RHM].[ManufactureCode] = @ManufactureCode
						  OR ISNULL(@ManufactureCode,0) = 0)
					 AND ([RHM].[BrandId] = @BrandId
						  OR ISNULL(@BrandId,0) = 0)
					 AND ([RHM].[DefaultPortNumber] = @DefaultPortNumber
						  OR ISNULL(@DefaultPortNumber,0) = 0)
					 AND ([RHM].[DefaultCapacity] = @DefaultCapacity
						  OR ISNULL(@DefaultCapacity,0) = 0)
			ORDER BY [RHM].Id
            OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
		END
		ELSE
			BEGIN
				SELECT	[RHM].[Id] AS Data_Id,
					[RHM].[Name] AS Data_Name,
					[RHM].[ManufactureCode] AS Data_ManufactureCode,
					[RHM].[DefaultPortNumber] AS Data_DefaultPortNumber,
					[RHM].[DefaultCapacity] AS Data_DefaultCapacity,
					[RHM].[Description] AS Data_Description,
				    [RHBrand].[code] AS  Data_Brand_code,
				    [RHBrand].[Name] AS  Data_Brand_Name,
				    [RHBrand].[OrderIndex] AS  Data_Brand_OrderIndex,
				    [RHBrand].[Description] AS  Data_Brand_Description,
				   1 AS [from],
				   1 AS PageNumber,
				   count(*) OVER () AS PageSize,
				   count(*) OVER () AS [Count],
                   @Message AS e_Message,
                   @Code AS e_Code,
                   @Validate AS e_Validate
				FROM [Rly].[RelayHubModel] AS RHM
			INNER JOIN
            [dbo].[lookup] AS RHBrand
            ON RHM.BrandId = RHBrand.code
			WHERE   ([RHM].[Id] = @Id
						  OR ISNULL(@Id,'') = '')
					 AND ([RHM].Name = @Name
						  OR ISNULL(@Name,'') = '')
					 AND ([RHM].[ManufactureCode] = @ManufactureCode
						  OR ISNULL(@ManufactureCode,0) = 0)
					 AND ([RHM].[BrandId] = @BrandId
						  OR ISNULL(@BrandId,0) = 0)
					 AND ([RHM].[DefaultPortNumber] = @DefaultPortNumber
						  OR ISNULL(@DefaultPortNumber,0) = 0)
					 AND ([RHM].[DefaultCapacity] = @DefaultCapacity
						  OR ISNULL(@DefaultCapacity,0) = 0)
			END
	 
END
