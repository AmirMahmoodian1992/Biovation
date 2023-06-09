Create PROCEDURE [dbo].[SelectCameraModel]
@Id INT=NULL, @Name NVARCHAR (MAX)=NULL, @ManufactureCode INT=NULL, @BrandCode INT=NULL, @PageSize INT=0, @PageNumber INT=0
AS
DECLARE @HasPaging AS BIT;
DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (15) = N'200';
BEGIN
    SET @HasPaging = CASE WHEN @PageSize = 0
                               AND @PageNumber = 0 THEN 0 ELSE 1 END;
    IF @HasPaging = 1
        BEGIN
            SELECT   [CM].Id AS Data_Id,
                     [CM].Name AS Data_Name,
                     [CM].ManufactureCode AS Data_ManufactureCode,
                     [CM].Description AS Data_Description,
                     [CM].DefaultPortNumber AS Data_DefaultPortNumber,
                     [CM].DefaultUserName AS Data_DefaultUserName,
                     [CM].DefaultPassword AS Data_DefaultPassword,
                     [B].Code AS Data_Brand_Code,
                     [B].Name AS Data_Brand_Name,
                     [B].Description AS Data_Brand_Description,
                     [PRA].Id AS Data_ProtocolRemainAddresses_Id,
                     [PRA].OrderIndex AS Data_ProtocolRemainAddresses_OrderIndex,
                     [PRA].RemainAddress AS Data_ProtocolRemainAddresses_RemainAddress,
                     [L].Code AS Data_ProtocolRemainAddresses_Protocol_Code,
                     [L].Name AS Data_ProtocolRemainAddresses_Protocol_Name,
                     [L].Description AS Data_ProtocolRemainAddresses_Protocol_Description,
                     [L].[LookupCategoryId] AS Data_ProtocolRemainAddresses_Protocol_Category_Id,
                     (@PageNumber - 1) * @PageSize AS [from],
                     @PageNumber AS PageNumber,
                     @PageSize AS PageSize,
                     count(*) OVER () AS [Count],
                     @Message AS Message,
                     @Code AS Code,
                     @Validate AS Validate
            FROM     [Rly].[CameraModel] AS CM
                     LEFT JOIN
                     [dbo].Lookup AS B
                     ON [B].[Code] = [CM].[BrandCode]
                     LEFT OUTER JOIN
                     [Rly].ProtocolRemainAddressesCameraModel AS PRAM
                     ON [PRAM].CameraModelId = [CM].Id
                     LEFT OUTER JOIN
                     [Rly].ProtocolRemainAddresses AS PRA
                     ON [PRA].Id = [PRAM].ProtocolRemainAddressesId
                     LEFT JOIN
                     [dbo].Lookup AS L
                     ON [L].Code = [PRA].ProtocolCode
            WHERE    ([CM].[Id] = @Id
                      OR ISNULL(@Id, 0) = 0)
                     AND ([CM].[Name] = @Name
                          OR ISNULL(@Name, '') = '')
                     AND ([CM].[ManufactureCode] = @ManufactureCode
                          OR ISNULL(@ManufactureCode, 0) = 0)
                     AND ([CM].[BrandCode] = @BrandCode
                          OR ISNULL(@BrandCode, 0) = 0)
            ORDER BY [CM].[Id]
            OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
        END
    ELSE
        BEGIN
            SELECT [CM].Id AS Data_Id,
                   [CM].Name AS Data_Name,
                   [CM].ManufactureCode AS Data_ManufactureCode,
                   [CM].Description AS Data_Description,
                   [CM].DefaultPortNumber AS Data_DefaultPortNumber,
                   [CM].DefaultUserName AS Data_DefaultUserName,
                   [CM].DefaultPassword AS Data_DefaultPassword,
                   [B].Code AS Data_Brand_Code,
                   [B].Name AS Data_Brand_Name,
                   [B].Description AS Data_Brand_Description,
                   [PRA].Id AS Data_ProtocolRemainAddresses_Id,
                   [PRA].OrderIndex AS Data_ProtocolRemainAddresses_OrderIndex,
                   [PRA].RemainAddress AS Data_ProtocolRemainAddresses_RemainAddress,
                   [L].Code AS Data_ProtocolRemainAddresses_Protocol_Code,
                   [L].Name AS Data_ProtocolRemainAddresses_Protocol_Name,
                   [L].Description AS Data_ProtocolRemainAddresses_Protocol_Description,
                   [L].[LookupCategoryId] AS Data_ProtocolRemainAddresses_Protocol_Category_Id,
                   1 AS [from],
                   1 AS PageNumber,
                   count(*) OVER () AS PageSize,
                   count(*) OVER () AS [Count],
                   @Message AS Message,
                   @Code AS Code,
                   @Validate AS Validate
            FROM   [Rly].[CameraModel] AS CM
                   LEFT JOIN
                   [dbo].Lookup AS B
                   ON [B].[Code] = [CM].[BrandCode]
                   LEFT OUTER JOIN
                   [Rly].ProtocolRemainAddressesCameraModel AS PRAM
                   ON [PRAM].CameraModelId = [CM].Id
                   LEFT OUTER JOIN
                   [Rly].ProtocolRemainAddresses AS PRA
                   ON [PRA].Id = [PRAM].ProtocolRemainAddressesId
                   LEFT JOIN
                   [dbo].Lookup AS L
                   ON [L].Code = [PRA].ProtocolCode
            WHERE  ([CM].[Id] = @Id
                    OR ISNULL(@Id, 0) = 0)
                   AND ([CM].[Name] = @Name
                        OR ISNULL(@Name, '') = '')
                   AND ([CM].[ManufactureCode] = @ManufactureCode
                        OR ISNULL(@ManufactureCode, 0) = 0)
                   AND ([CM].[BrandCode] = @BrandCode
                        OR ISNULL(@BrandCode, 0) = 0);
        END
END