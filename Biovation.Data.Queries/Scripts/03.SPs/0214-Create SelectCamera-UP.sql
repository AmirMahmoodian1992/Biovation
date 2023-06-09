 CREATE PROCEDURE [dbo].[SelectCamera]
@AdminUserId BIGINT=NULL, @Id BIGINT=NULL, @Ip NVARCHAR (50)=NULL, @Port INT=NULL, @Name NVARCHAR (MAX) = NULL, @Code INT=NULL, @ModelId INT=NULL, @BrandCode INT=NULL, @PageNumber INT=0, @PageSize INT=0, @Where NVARCHAR (MAX)='', @Order NVARCHAR (MAX)='' , @FilterText NVARCHAR (MAX) = NULL
AS
DECLARE @HasPaging AS BIT;
DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1,@CameraCode  AS NVARCHAR (5) = N'200';
BEGIN
    SET @HasPaging = CASE WHEN @PageSize = 0
                               AND @PageNumber = 0 THEN 0 ELSE 1 END;
	DECLARE @CountSqlQuery AS NVARCHAR (MAX);
    DECLARE @DataSqlQuery AS NVARCHAR (MAX);
    DECLARE @ParmDefinition AS NVARCHAR (500);
	SET @where = REPLACE(@where, 'CameraId', 'Id');
	SET @where = REPLACE(@where, 'CameraName', 'Name');
    SET @where = REPLACE(@where, 'True', '1');
    SET @where = REPLACE(@where, 'False', '0');
    SET @order = REPLACE(@order, 'CameraId', 'Data_Id');
    SET @Order = REPLACE(@order, 'CameraId', 'Data_Name');
	SET @Order = REPLACE(@order, 'Active', 'Data_Active');
	IF ISNULL(@Order,N'') = N''
		BEGIN
			SET	@Order = N'ORDER BY Data_Id ASC';
		END
    IF @HasPaging = 1
        BEGIN
		SET @DataSqlQuery = N'
            SELECT   [C].[Id] AS Data_Id,
				     [C].[Code] AS Data_Code,
                     [C].[Ip] AS Data_ConnectionInfo_Ip,
                     [C].[Port] AS Data_ConnectionInfo_Port,
                     [C].[UserName] AS Data_ConnectionInfo_UserName,
					 [C].[Password] AS Data_ConnectionInfo_Password,
					 [C].[MacAddress] AS Data_ConnectionInfo_MacAddress,
					 [C].[Name] AS Data_Name,
                     [C].[Active] AS Data_Active,
					 [C].[RegisterDate] AS Data_RegisterDate,
					 [C].[HardwareVersion] AS Data_HardwareVersion,
					 [C].[SerialNumber] AS Data_SerialNumber,
					 [LO].Code AS Data_Brand_Code,
					 [LO].[Name] AS Data_Brand_Name,
					 [LO].[Description] AS Data_Brand_Description,
					 [C].ConnectionUrl AS Data_ConnectionUrl,
					 [C].LiveStreamUrl AS Data_LiveStreamUrl,
					 [CM].Id AS Data_Model_Id,
					[CM].Name AS Data_Model_Name,
					[CM].ManufactureCode AS Data_Model_ManufactureCode,
					[CM].Description AS Data_Model_Description,
					[CM].DefaultPortNumber AS Data_Model_DefaultPortNumber,
					[CM].DefaultUserName AS Data_Model_DefaultUserName,
					[CM].DefaultPassword AS Data_Model_DefaultPassword,
					[B].Code AS Data_Model_Brand_Code,
					[B].Name AS Data_Model_Brand_Name,
					[B].Description AS Data_Model_Brand_Description,
					[PRA].Id AS Data_Model_ProtocolRemainAddresses_Id,
					[PRA].OrderIndex AS Data_Model_ProtocolRemainAddresses_OrderIndex,
					[PRA].RemainAddress AS Data_Model_ProtocolRemainAddresses_RemainAddress,
					[L].Code AS Data_Model_ProtocolRemainAddresses_Protocol_Code,
					[L].Name AS Data_Model_ProtocolRemainAddresses_Protocol_Name,
					[L].Description AS Data_Model_ProtocolRemainAddresses_Protocol_Description,
					[R].Code AS Data_Resolution_Code,
					[R].Name AS Data_Resolution_Name,
					[R].Description AS Data_Resolution_Description,
                     (@PageNumber - 1) * @PageSize AS [from],
                     @PageNumber AS PageNumber,
                     @PageSize AS PageSize,
                     count(*) OVER () AS [Count],
                     @Message AS e_Message,
                     @CameraCode AS e_Code,
                     @Validate AS e_Validate
            FROM     [Rly].Camera AS C
                     LEFT OUTER JOIN
                     [Rly].CameraModel AS CM
                     ON [C].ModelId = [CM].Id
					 LEFT JOIN
					 [dbo].Lookup AS LO ON
					 [LO].Code = [C].[BrandCode]
					 LEFT JOIN
					[dbo].Lookup AS B ON
					[B].[Code] = [CM].[BrandCode]
					LEFT OUTER JOIN
					[Rly].ProtocolRemainAddressesCameraModel AS PRAM ON
					[PRAM].CameraModelId = [CM].Id
					LEFT OUTER JOIN
					[Rly].ProtocolRemainAddresses AS PRA ON
					[PRA].Id = [PRAM].ProtocolRemainAddressesId
					LEFT JOIN
					[dbo].Lookup AS L ON
					[L].Code = [PRA].ProtocolCode
					LEFT JOIN
					[dbo].Lookup AS R ON
					[R].[Code] = [C].[ResolutionCode]

            WHERE    (isnull(@AdminUserId, 0) = 0
                      OR EXISTS (SELECT Id
                                 FROM   [dbo].[USER]
                                 WHERE  IsMasterAdmin = 1
                                        AND ID = @AdminUserId))
                     AND ([C].Id = @Id
                          OR ISNULL(@Id, 0) = 0)
                     AND ([C].[Ip] = @Ip
                          OR ISNULL(@Ip, '''') = '''')
                     AND ([C].[Port] = @Port
                          OR ISNULL(@Port, 0) = 0)
                     AND ([C].[Name] = @Name
                          OR ISNULL(@Name, '''') = '''')
                     AND ([C].[Code] = @Code
                          OR ISNULL(@Code, 0) = 0)
                     AND ([C].[ModelId] = @ModelId
                          OR ISNULL(@ModelId, 0) = 0)
					 AND ([C].[BrandCode] = @BrandCode
                          OR ISNULL(@BrandCode, 0) = 0)
					 AND ((@FilterText IS NULL)
					   OR ([C].[Name] LIKE ''%'' + @FilterText + ''%'')
					   OR ([c].[UserName] LIKE ''%'' + @FilterText + ''%'')
					   OR ([B].Name LIKE ''%'' + @FilterText + ''%'')
					   OR ([CM].Name LIKE ''%'' + @FilterText + ''%'')
					   OR ([C].[Code] LIKE ''%'' + @FilterText + ''%''))
					' + ISNULL(@Where,'') + @Order + '
			OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;'
			SET @ParmDefinition = N'@AdminUserId INT,@Id int, @Ip NVARCHAR (50), @Port INT, @Code BigInt, @Name nvarchar(MAX), @ModelId INT, @BrandCode INT
								,@FilterText NVARCHAR(MAX), @PageNumber int, @PageSize int, @Message AS NVARCHAR (200), @Validate int, @CameraCode nvarchar(5)';
            EXECUTE sp_executesql @DataSqlQuery, @ParmDefinition, @AdminUserId = @AdminUserId ,@Id = @Id, @Ip = @Ip, @Port = @Port, @Code = @Code, @Name = @Name, @ModelId = @ModelId, @BrandCode= @BrandCode, @FilterText = @FilterText, @PageNumber = @PageNumber, @PageSize = @PageSize, @Message = @Message, @Validate = @Validate, @CameraCode = @CameraCode;

        END
    ELSE
        BEGIN
           SET @DataSqlQuery = N' SELECT [C].[Id] AS Data_Id,
				     [C].[Code] AS Data_Code,
                     [C].[Ip] AS Data_ConnectionInfo_Ip,
                     [C].[Port] AS Data_ConnectionInfo_Port,
                     [C].[UserName] AS Data_ConnectionInfo_UserName,
					 [C].[Password] AS Data_ConnectionInfo_Password,
					 [C].[MacAddress] AS Data_ConnectionInfo_MacAddress,
					 [C].[Name] AS Data_Name,
                     [C].[Active] AS Data_Active,
					 [C].[RegisterDate] AS Data_RegisterDate,
					 [C].[HardwareVersion] AS Data_HardwareVersion,
					 [C].[SerialNumber] AS Data_SerialNumber,
					 [LO].Code AS Data_Brand_Code,
					 [LO].[Name] AS Data_Brand_Name,
					 [LO].[Description] AS Data_Brand_Description,
					 [C].ConnectionUrl AS Data_ConnectionUrl,
					 [C].LiveStreamUrl AS Data_LiveStreamUrl,
					 [CM].Id AS Data_Model_Id,
					[CM].Name AS Data_Model_Name,
					[CM].ManufactureCode AS Data_Model_ManufactureCode,
					[CM].Description AS Data_Model_Description,
					[CM].DefaultPortNumber AS Data_Model_DefaultPortNumber,
					[CM].DefaultUserName AS Data_Model_DefaultUserName,
					[CM].DefaultPassword AS Data_Model_DefaultPassword,
					[B].Code AS Data_Model_Brand_Code,
					[B].Name AS Data_Model_Brand_Name,
					[B].Description AS Data_Model_Brand_Description,
					[PRA].Id AS Data_Model_ProtocolRemainAddresses_Id,
					[PRA].OrderIndex AS Data_Model_ProtocolRemainAddresses_OrderIndex,
					[PRA].RemainAddress AS Data_Model_ProtocolRemainAddresses_RemainAddress,
					[L].Code AS Data_Model_ProtocolRemainAddresses_Protocol_Code,
					[L].Name AS Data_Model_ProtocolRemainAddresses_Protocol_Name,
					[L].Description AS Data_Model_ProtocolRemainAddresses_Protocol_Description,
					[R].Code AS Data_Resolution_Code,
					[R].Name AS Data_Resolution_Name,
					[R].Description AS Data_Resolution_Description,
                   1 AS [from],
                   1 AS PageNumber,
                   count(*) OVER () AS PageSize,
                   count(*) OVER () AS [Count],
                   @Message AS e_Message,
                   @CameraCode AS e_Code,
                   @Validate AS e_Validate
            FROM   [Rly].Camera AS C
                     LEFT OUTER JOIN
                     [Rly].CameraModel AS CM
                     ON [C].ModelId = [CM].Id
					 LEFT JOIN
					 [dbo].Lookup AS LO ON
					 [LO].Code = [C].[BrandCode]
					 LEFT JOIN
					[dbo].Lookup AS B ON
					[B].[Code] = [CM].[BrandCode]
					LEFT OUTER JOIN
					[Rly].ProtocolRemainAddressesCameraModel AS PRAM ON
					[PRAM].CameraModelId = [CM].Id
					LEFT OUTER JOIN
					[Rly].ProtocolRemainAddresses AS PRA ON
					[PRA].Id = [PRAM].ProtocolRemainAddressesId
					LEFT JOIN
					[dbo].Lookup AS L ON
					[L].Code = [PRA].ProtocolCode
					LEFT JOIN
					[dbo].Lookup AS R ON
					[R].[Code] = [C].[ResolutionCode]
            WHERE  (isnull(@AdminUserId, 0) = 0
                      OR EXISTS (SELECT Id
                                 FROM   [dbo].[USER]
                                 WHERE  IsMasterAdmin = 1
                                        AND ID = @AdminUserId))
                     AND ([C].Id = @Id
                          OR ISNULL(@Id, 0) = 0)
                     AND ([C].[Ip] = @Ip
                          OR ISNULL(@Ip, '''') = '''')
                     AND ([C].[Port] = @Port
                          OR ISNULL(@Port, 0) = 0)
                     AND ([C].[Name] = @Name
                          OR ISNULL(@Name, '''') = '''')
                     AND ([C].[Code] = @Code
                          OR ISNULL(@Code, 0) = 0)
                     AND ([C].[ModelId] = @ModelId
                          OR ISNULL(@ModelId, 0) = 0)
					 AND ([C].[BrandCode] = @BrandCode
                          OR ISNULL(@BrandCode, 0) = 0)
					 AND ((@FilterText IS NULL)
					   OR ([C].[Name] LIKE ''%'' + @FilterText + ''%'')
					   OR ([c].[UserName] LIKE ''%'' + @FilterText + ''%'')
					   OR ([B].Name LIKE ''%'' + @FilterText + ''%'')
					   OR ([CM].Name LIKE ''%'' + @FilterText + ''%'')
					   OR ([C].[Code] LIKE ''%'' + @FilterText + ''%''))' +  ISNULL(@Where,'') + @Order
			SET @ParmDefinition = N'@AdminUserId INT,@Id int, @Ip NVARCHAR (50), @Port INT, @Code BigInt, @Name nvarchar(MAX), @ModelId INT, @BrandCode INT
								,@FilterText NVARCHAR(MAX), @Message AS NVARCHAR (200), @Validate int, @CameraCode nvarchar(5)';
            EXECUTE sp_executesql @DataSqlQuery, @ParmDefinition, @AdminUserId = @AdminUserId ,@Id = @Id, @Ip = @Ip, @Port = @Port, @Code = @Code, @Name = @Name, @ModelId = @ModelId, @BrandCode= @BrandCode, @FilterText = @FilterText, @Message = @Message, @Validate = @Validate, @CameraCode = @CameraCode;

        END
END