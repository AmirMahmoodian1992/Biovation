Create PROCEDURE [dbo].[SelectPlateDetectionLogs]
 @FirstLicensePLatePart nvarchar(10)= Null,
 @SecondLicensePLatePart nvarchar(10)= Null,
 @ThirdLicensePLatePart nvarchar(10)= Null,
 @FourthLicensePLatePart nvarchar(10)= Null,
 @LogId INT=0,
 @LicensePlate nvarchar(20)=NULL,
 @DetectorId INT =NULL,
 @SuccessTransfer BIT = NULL, 
 @FromDate DATETIME = NULL, 
 @ToDate DATETIME = NULL,
 @MinPrecision tinyint = 0,
 @MaxPrecision  tinyint = 0,
 @InOrOut TINYINT = 0,
 @adminUserId INT=0,
 @WithPic bit = 0,
 @PageNumber AS INT=0,
 @PageSize AS INT =0,
 @Where nvarchar(MAX) = '',
 @Order nvarchar(MAX) = ''
AS
          DECLARE  @HasPaging   BIT;
		  DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS nvarchar(5) = N'200';
   
BEGIN
	DECLARE @total AS INT;
 SET @HasPaging = CASE
                                 WHEN @PageSize =0
                                      AND @PageNumber =0 THEN
                                     0
                                 ELSE
                                     1
                             END;
    
	DECLARE @CountSqlQuery AS NVARCHAR (MAX);
	DECLARE @DataSqlQuery AS NVARCHAR (MAX);
	DECLARE @ParmDefinition AS NVARCHAR (500);

	SET @where = REPLACE(@where, 'DeviceCode', 'dev.Code');
	SET @where = REPLACE(@where, 'DeviceName', 'dev.Name');
	SET @where = REPLACE(@where, 'True', '1');
	SET @where = REPLACE(@where, 'False', '0');
	SET @where = REPLACE(@where, 'SLogDateTime', 'DateTime');
	SET @where = REPLACE(@where, 'UserCode', 'LicensePlate.LicensePlate');
	SET @where = REPLACE(@where, 'UserId', 'LicensePlate.LicensePlate');
	--SET @where = REPLACE(@where, 'MatchingTypeTitle', '[MatchingTypeLookup].[Description]');
	SET @Order = REPLACE(@order, 'DeviceCode', 'dev.Code');
	SET @Order = REPLACE(@order, 'DeviceName', 'dev.Name');
	SET @Order = REPLACE(@order, 'UserCode', 'LicensePlate.LicensePlate');
	SET @Order = REPLACE(@order, 'UserId', 'LicensePlate.LicensePlate');
	SET @Order = REPLACE(@order, 'SLogDateTime', 'DateTime');
	--SET @Order = REPLACE(@order, 'MatchingTypeTitle', '[MatchingTypeLookup].[Code]');

	SET @CountSqlQuery ='SELECT @total = COUNT(*)
  FROM [dbo].[PlateDetectionLog]  AS PL
           LEFT OUTER JOIN
                   [dbo].[Lookup] AS [LogEventLookUp]
                   ON PL.[EventId]  = [LogEventLookUp].[Code]
				   LEFT OUTER JOIN
				   [dbo].[Device] AS [Dev]
				   ON PL.[DetectorId]  = [Dev].[Id]
				   LEFT OUTER JOIN
				   [dbo].[LicensePlate] AS [LicensePlate]
				   ON PL.LicensePlateId  = [LicensePlate].[EntityId]

		     	WHERE    (PL.[DetectorId] = @DetectorId
						  OR COALESCE (@DetectorId, '''') = '''')
						 AND (PL.Id = @LogId
						  OR COALESCE (@logid, '''') = '''')
						 AND ( LicensePlate.[LicensePlate] = @LicensePlate
							  OR COALESCE (@LicensePlate, '''') = '''')
						 AND ( LicensePlate.[FirstPart] = @FirstLicensePLatePart
							  OR COALESCE ( @FirstLicensePLatePart, '''') = '''')
	   				     AND ( LicensePlate.[SecondPart] = @SecondLicensePLatePart
							  OR COALESCE ( @SecondLicensePLatePart, '''') = '''')
						 AND ( LicensePlate.[ThirdPart] = @ThirdLicensePLatePart
							  OR COALESCE ( @ThirdLicensePLatePart, '''') = '''')
						 AND ( LicensePlate.[FourthPart] = @FourthLicensePLatePart
										  OR COALESCE ( @FourthLicensePLatePart, '''') = '''')
						 AND (isnull(@fromDate, '''') = ''''
							  OR CONVERT (DATE, PL.[LogDateTime]) >= CONVERT (DATE, @FromDate))
						 AND (isnull(@todate, '''') = ''''
							  OR CONVERT (DATE, PL.[LogDateTime]) <= CONVERT (DATE, @ToDate))
						AND (isnull(@MinPrecision, 0) = 0
						     OR PL.[DetectionPrecision] >= @MinPrecision)
						 AND (isnull(@MaxPrecision, 0) = 0
							  OR PL.[DetectionPrecision] <= @MaxPrecision)
						 AND (isnull(@InOrOut, 0) = 0
							  OR PL.[InOrOut] = @InOrOut)
						 AND (Pl.SuccessTransfer = ISNULL(@SuccessTransfer, 1)
							  OR @SuccessTransfer IS NULL) ' + @Where

            SET @ParmDefinition = N'@DetectorId int, @LogId int, @LicensePlate nvarchar(20), @FirstLicensePLatePart nvarchar(10), @SecondLicensePLatePart nvarchar(10), @ThirdLicensePLatePart nvarchar(10), @FourthLicensePLatePart nvarchar(10), @MinPrecision int ,@MaxPrecision int, @InOrOut int, @FromDate datetime,@ToDate datetime,@SuccessTransfer bit,@total int OUTPUT';
            EXECUTE sp_executesql @CountSqlQuery, @ParmDefinition, @DetectorId, @LogId = @LogId, @LicensePlate = @LicensePlate , @FirstLicensePLatePart = @FirstLicensePLatePart, @SecondLicensePLatePart = @SecondLicensePLatePart, @ThirdLicensePLatePart = @ThirdLicensePLatePart, @FourthLicensePLatePart = @FourthLicensePLatePart, @MinPrecision = @MinPrecision, @MaxPrecision = @MaxPrecision, @InOrOut = @InOrOut, @FromDate = @FromDate, @ToDate = @ToDate, @SuccessTransfer = @SuccessTransfer, @total = @total OUTPUT;
            
            IF @Order = ''
                SET @Order = ' ORDER BY [LogDateTime] DESC';

	IF @HasPaging = 1
		BEGIN
			IF(@WithPic = 1)
						BEGIN
				SET @DataSqlQuery = 'SELECT 
				   @total as Total
				  ,PL.[Id] AS Data_Id
				  ,[DetectorId] AS Data_DetectorId
				  ,[DEV].[Name] AS Data_DeviceName
				  ,[LicensePlate].[EntityId] AS Data_LicensePlate_EntityId
				  ,[LicensePlate].[LicensePlate] AS Data_LicensePlate_LicensePlateNumber
				  ,[EventId] AS Data_EventLog_Code
				  ,[LogDateTime] AS Data_LogDateTime
				  ,[DetectionPrecision]  AS Data_DetectionPrecision
				  ,[LogPic].[FullImage] AS Data_FullImage
				  ,[LogPic].[PlateImage] AS Data_PlateImage
				  ,[SuccessTransfer] AS Data_SuccessTransfer
				  ,[InOrOut] AS Data_InOrOut
				  ,[LogEventLookUp].[Name] AS Data_EventLog_Name
				  ,[LogEventLookUp].[OrderIndex] AS Data_EventLog_OrderIndex
				  ,[LogEventLookUp].[Description] AS Data_EventLog_Description
				  ,(@PageNumber-1)*@PageSize  AS [from],
					  @PageNumber AS PageNumber,
					   @PageSize As PageSize,
					   count(*) OVER() AS [Count],
					   @Message AS e_Message,
					   @Code  AS e_Code,
					   @Validate  AS e_Validate
			  FROM [dbo].[PlateDetectionLog]  AS PL
					   LEFT OUTER JOIN
							   [dbo].[Lookup] AS [LogEventLookUp]
							   ON PL.[EventId]  = [LogEventLookUp].[Code]
							   LEFT OUTER JOIN
							   [dbo].[Device] AS [Dev]
							   ON PL.[DetectorId]  = [Dev].[Id]
							   LEFT OUTER JOIN
							   [dbo].[PlateDetectionPictureLog] AS [LogPic]
							   ON PL.[Id]  = [LogPic].[LogId]
							   LEFT OUTER JOIN
							   [dbo].[LicensePlate] AS [LicensePlate]
							   ON [LicensePlate].[EntityId] =  PL.LicensePlateId

		     				WHERE    (PL.[DetectorId] = @DetectorId
									  OR COALESCE (@DetectorId, '''') = '''')
									 AND (PL.Id = @LogId
									  OR COALESCE (@logid, '''') = '''')
									 AND ( LicensePlate.[LicensePlate] = @LicensePlate
										  OR COALESCE ( @LicensePlate, '''') = '''')
									 AND ( LicensePlate.[FirstPart] = @FirstLicensePLatePart
										  OR COALESCE ( @FirstLicensePLatePart, '''') = '''')
									 AND ( LicensePlate.[SecondPart] = @SecondLicensePLatePart
										  OR COALESCE ( @SecondLicensePLatePart, '''') = '''')
									 AND ( LicensePlate.[ThirdPart] = @ThirdLicensePLatePart
										  OR COALESCE ( @ThirdLicensePLatePart, '''') = '''')
									 AND ( LicensePlate.[FourthPart] = @FourthLicensePLatePart
										  OR COALESCE ( @FourthLicensePLatePart, '''') = '''')
									 AND (isnull(@fromDate, '''') = ''''
										  OR CONVERT (DATE, PL.[LogDateTime]) >= CONVERT (DATE, @FromDate))
									 AND (isnull(@todate, '''') = ''''
										  OR CONVERT (DATE, PL.[LogDateTime]) <= CONVERT (DATE, @ToDate))
									AND (isnull(@MinPrecision, 0) = 0
										 OR PL.[DetectionPrecision] >= @MinPrecision)
									 AND (isnull(@MaxPrecision, 0) = 0
										  OR PL.[DetectionPrecision] <= @MinPrecision)
									AND (isnull(@InOrOut, 0) = 0
										  OR PL.[InOrOut] = @InOrOut)
									 AND (Pl.SuccessTransfer = ISNULL(@SuccessTransfer, 1)
										  OR @SuccessTransfer IS NULL) ' + @Where + '
							/*ORDER BY [LogDateTime] DESC*/ ' + @Order + '
							OFFSET (@PageNumber-1)*@PageSize ROWS
							FETCH NEXT @PageSize ROWS ONLY'

            SET @ParmDefinition = N'@DetectorId int, @LogId int, @LicensePlate nvarchar(20), @FirstLicensePLatePart nvarchar(10), @SecondLicensePLatePart nvarchar(10), @ThirdLicensePLatePart nvarchar(10), @FourthLicensePLatePart nvarchar(10), @MinPrecision int ,@MaxPrecision int, @InOrOut int, @FromDate datetime,@ToDate datetime
										,@SuccessTransfer bit,@total int, @PageNumber int, @PageSize int, @Message AS NVARCHAR (200), @Validate int, @Code nvarchar(5)';
            EXECUTE sp_executesql @DataSqlQuery, @ParmDefinition, @DetectorId, @LogId = @LogId, @LicensePlate = @LicensePlate, @FirstLicensePLatePart = @FirstLicensePLatePart, @SecondLicensePLatePart = @SecondLicensePLatePart, @ThirdLicensePLatePart = @ThirdLicensePLatePart, @FourthLicensePLatePart = @FourthLicensePLatePart, @MinPrecision = @MinPrecision, @MaxPrecision = @MaxPrecision, @InOrOut = @InOrOut, @FromDate = @FromDate, @ToDate = @ToDate
			, @SuccessTransfer = @SuccessTransfer, @total = @total, @PageNumber = @PageNumber, @PageSize = @PageSize, @Message = @Message, @Validate = @Validate, @Code = @Code;
            
			END 	   
			ELSE
					BEGIN 
					SET @DataSqlQuery = 'SELECT  
				   @total as Total
				  ,PL.[Id] AS Data_Id
				  ,[DetectorId] AS Data_DetectorId
				  ,[DEV].[Name] AS Data_DeviceName
				  ,[LicensePlate].[EntityId] AS Data_LicensePlate_EntityId
				  ,[LicensePlate].[LicensePlate] AS Data_LicensePlate_LicensePlateNumber
				  ,[EventId] AS Data_EventLog_Code
				  ,[LogDateTime] AS Data_LogDateTime
				  ,[DetectionPrecision]  AS Data_DetectionPrecision
				  ,[SuccessTransfer] AS Data_SuccessTransfer
				  ,[InOrOut] AS Data_InOrOut
				  ,[LogEventLookUp].[Name] AS Data_EventLog_Name
				  ,[LogEventLookUp].[OrderIndex] AS Data_EventLog_OrderIndex
				  ,[LogEventLookUp].[Description] AS Data_EventLog_Description,
					   1 AS [from],
					   1 AS PageNumber,
					   count(*) OVER() As PageSize,
					   count(*) OVER() AS [Count],
					   @Message AS e_Message,
					   @Code  AS e_Code,
					   @Validate  AS e_Validate
			  FROM [dbo].[PlateDetectionLog]  AS PL
					   LEFT OUTER JOIN
							   [dbo].[Lookup] AS [LogEventLookUp]
							   ON PL.[EventId]  = [LogEventLookUp].[Code]
							   LEFT OUTER JOIN
							   [dbo].[Device] AS [Dev]
							   ON PL.[DetectorId]  = [Dev].[Id]
							   LEFT OUTER JOIN
							   [dbo].[LicensePlate] AS [LicensePlate]
							   ON PL.LicensePlateId  = [LicensePlate].[EntityId]

		     				WHERE    (PL.[DetectorId] = @DetectorId
									  OR COALESCE (@DetectorId, '''') = '''')
									 AND (PL.Id = @LogId
									  OR COALESCE (@logid, '''') = '''')
									 AND ( LicensePlate.[LicensePlate] = @LicensePlate
										  OR COALESCE (@LicensePlate, '''') = '''')
									 AND ( LicensePlate.[FirstPart] = @FirstLicensePLatePart
										  OR COALESCE ( @FirstLicensePLatePart, '''') = '''')
									 AND ( LicensePlate.[SecondPart] = @SecondLicensePLatePart
										  OR COALESCE ( @SecondLicensePLatePart, '''') = '''')
									 AND ( LicensePlate.[ThirdPart] = @ThirdLicensePLatePart
										  OR COALESCE ( @ThirdLicensePLatePart, '''') = '''')
									 AND ( LicensePlate.[FourthPart] = @FourthLicensePLatePart
										  OR COALESCE ( @FourthLicensePLatePart, '''') = '''')
									 AND (isnull(@fromDate, '''') = ''''
										  OR CONVERT (DATE, PL.[LogDateTime]) >= CONVERT (DATE, @FromDate))
									 AND (isnull(@todate, '''') = ''''
										  OR CONVERT (DATE, PL.[LogDateTime]) <= CONVERT (DATE, @ToDate))
								   	 AND (isnull(@MinPrecision, 0) = 0
										 OR PL.[DetectionPrecision] >= @MinPrecision)
									 AND (isnull(@MaxPrecision, 0) = 0
										  OR PL.[DetectionPrecision] <= @MaxPrecision)
									AND (isnull(@InOrOut, 0) = 0
										  OR PL.[InOrOut] = @InOrOut)
									 AND (Pl.SuccessTransfer = ISNULL(@SuccessTransfer, 1)
										  OR @SuccessTransfer IS NULL) ' + @Where + '
							/*ORDER BY [LogDateTime] DESC*/ ' + @Order + ' 
							OFFSET (@PageNumber-1)*@PageSize ROWS
							FETCH NEXT @PageSize ROWS ONLY'
            
			SET @ParmDefinition = N'@DetectorId int, @LogId int, @LicensePlate nvarchar(20),@FirstLicensePLatePart nvarchar(10), @SecondLicensePLatePart nvarchar(10), @ThirdLicensePLatePart nvarchar(10), @FourthLicensePLatePart nvarchar(10),  @MinPrecision int ,@MaxPrecision int, @InOrOut int, @FromDate datetime,@ToDate datetime
										,@SuccessTransfer bit,@total int, @PageNumber int, @PageSize int, @Message AS NVARCHAR (200), @Validate int, @Code nvarchar(5)';
            EXECUTE sp_executesql @DataSqlQuery, @ParmDefinition, @DetectorId, @LogId = @LogId, @LicensePlate = @LicensePlate, @FirstLicensePLatePart = @FirstLicensePLatePart, @SecondLicensePLatePart = @SecondLicensePLatePart, @ThirdLicensePLatePart = @ThirdLicensePLatePart, @FourthLicensePLatePart = @FourthLicensePLatePart, @MinPrecision = @MinPrecision, @MaxPrecision = @MaxPrecision, @InOrOut = @InOrOut, @FromDate = @FromDate, @ToDate = @ToDate
			, @SuccessTransfer = @SuccessTransfer, @total = @total, @PageNumber = @PageNumber, @PageSize = @PageSize, @Message = @Message, @Validate = @Validate, @Code = @Code;
            
			END
		END
	ELSE
	BEGIN
		IF(@WithPic = 1)
					BEGIN
			SET @DataSqlQuery = 'SELECT @total AS Data_Total
			  ,[PL].[Id] AS Data_Id
			  ,[DetectorId] AS Data_DetectorId
			  ,[DEV].[Name] AS Data_DeviceName
			  ,[LicensePlate].[EntityId] AS Data_LicensePlate_EntityId
			  ,[LicensePlate].[LicensePlate] AS Data_LicensePlate_LicensePlateNumber
			  ,[EventId] AS Data_EventLog_Code
			  ,[LogDateTime] AS Data_LogDateTime
			  ,[DetectionPrecision] AS Data_DetectionPrecision
			  ,[LogPic].[FullImage] AS Data_FullImage
			  ,[LogPic].[PlateImage] AS Data_PlateImage
			  ,[SuccessTransfer] AS Data_SuccessTransfer
			  ,[InOrOut] AS Data_InOrOut
			  ,[LogEventLookUp].[Name] AS Data_EventLog_Name
			  ,[LogEventLookUp].[OrderIndex] AS Data_EventLog_OrderIndex
			  ,[LogEventLookUp].[Description] AS Data_EventLog_Description
		  FROM [dbo].[PlateDetectionLog]  AS PL
				   LEFT OUTER JOIN
						   [dbo].[Lookup] AS [LogEventLookUp]
						   ON PL.[EventId]  = [LogEventLookUp].[Code]
						   LEFT OUTER JOIN
						   [dbo].[Device] AS [Dev]
						   ON PL.[DetectorId]  = [Dev].[Id]
						   LEFT OUTER JOIN
						   [dbo].[PlateDetectionPictureLog] AS [LogPic]
						   ON PL.[Id]  = [LogPic].[LogId]
						   LEFT OUTER JOIN
						   [dbo].[LicensePlate] AS [LicensePlate]
						   ON [LicensePlate].[EntityId] =  PL.LicensePlateId

		     			WHERE    (PL.[DetectorId] = @DetectorId
								  OR COALESCE (@DetectorId, '''') = '''')
								 AND (PL.Id = @LogId
								  OR COALESCE (@logid, '''') = '''')
								 AND ( LicensePlate.[LicensePlate] = @LicensePlate
									  OR COALESCE ( @LicensePlate, '''') = '''')
								AND ( LicensePlate.[FirstPart] = @FirstLicensePLatePart
									  OR COALESCE ( @FirstLicensePLatePart, '''') = '''')
								AND ( LicensePlate.[SecondPart] = @SecondLicensePLatePart
									  OR COALESCE ( @SecondLicensePLatePart, '''') = '''')
								AND ( LicensePlate.[ThirdPart] = @ThirdLicensePLatePart
									  OR COALESCE ( @ThirdLicensePLatePart, '''') = '''')
								AND ( LicensePlate.[FourthPart] = @FourthLicensePLatePart
									  OR COALESCE ( @FourthLicensePLatePart, '''') = '''')
								 AND (isnull(@fromDate, '''') = ''''
									  OR CONVERT (DATE, PL.[LogDateTime]) >= CONVERT (DATE, @FromDate))
								 AND (isnull(@todate, '''') = ''''
									  OR CONVERT (DATE, PL.[LogDateTime]) <= CONVERT (DATE, @ToDate))
								AND (isnull(@MinPrecision, 0) = 0
									 OR PL.[DetectionPrecision] >= @MinPrecision)
								 AND (isnull(@MaxPrecision, 0) = 0
									  OR PL.[DetectionPrecision] <= @MaxPrecision)
								AND (isnull(@InOrOut, 0) = 0
									 OR PL.[InOrOut] = @InOrOut)
								 AND (Pl.SuccessTransfer = ISNULL(@SuccessTransfer, 1)
									  OR @SuccessTransfer IS NULL) ' + @Where + '
						/*ORDER BY [LogDateTime] DESC*/ ' + @Order
			
			SET @ParmDefinition = N'@DetectorId int, @LogId int, @LicensePlate nvarchar(20),@FirstLicensePLatePart nvarchar(10), @SecondLicensePLatePart nvarchar(10), @ThirdLicensePLatePart nvarchar(10), @FourthLicensePLatePart nvarchar(10),  @MinPrecision int ,@MaxPrecision int, @InOrOut int, @FromDate datetime,@ToDate datetime
										,@SuccessTransfer bit,@total int, @Message AS NVARCHAR (200), @Validate int, @Code nvarchar(5)';
            EXECUTE sp_executesql @DataSqlQuery, @ParmDefinition, @DetectorId, @LogId = @LogId, @LicensePlate = @LicensePlate, @FirstLicensePLatePart = @FirstLicensePLatePart, @SecondLicensePLatePart = @SecondLicensePLatePart, @ThirdLicensePLatePart = @ThirdLicensePLatePart, @FourthLicensePLatePart = @FourthLicensePLatePart, @MinPrecision = @MinPrecision, @MaxPrecision = @MaxPrecision, @InOrOut = @InOrOut, @FromDate = @FromDate, @ToDate = @ToDate
			, @SuccessTransfer = @SuccessTransfer, @total = @total, @Message = @Message, @Validate = @Validate, @Code = @Code;
            
				END 	   
		ELSE
				BEGIN 
				SET @DataSqlQuery = 'SELECT @total as Total
			   ,[PL].[Id] AS Data_Id
			  ,[DetectorId] AS Data_DetectorId
			  ,[DEV].[Name] AS Data_DeviceName
			  ,[LicensePlate].[EntityId] AS Data_LicensePlate_EntityId
			  ,[LicensePlate].[LicensePlate] AS Data_LicensePlate_LicensePlateNumber
			  ,[EventId] AS Data_EventLog_Code
			  ,[LogDateTime] AS Data_LogDateTime
			  ,[DetectionPrecision] AS Data_DetectionPrecision
			  ,[LogPic].[FullImage] AS Data_FullImage
			  ,[LogPic].[PlateImage] AS Data_PlateImage
			  ,[SuccessTransfer] AS Data_SuccessTransfer
			  ,[InOrOut] AS Data_InOrOut
			  ,[LogEventLookUp].[Name] AS Data_EventLog_Name
			  ,[LogEventLookUp].[OrderIndex] AS Data_EventLog_OrderIndex
			  ,[LogEventLookUp].[Description] AS Data_EventLog_Description
		  FROM [dbo].[PlateDetectionLog]  AS PL
				   LEFT OUTER JOIN
						   [dbo].[Lookup] AS [LogEventLookUp]
						   ON PL.[EventId]  = [LogEventLookUp].[Code]
						   LEFT OUTER JOIN
						   [dbo].[Device] AS [Dev]
						   ON PL.[DetectorId]  = [Dev].[Id]
						   LEFT OUTER JOIN
						   [dbo].[LicensePlate] AS [LicensePlate]
						   ON PL.LicensePlateId  = [LicensePlate].[EntityId]

		     			WHERE    (PL.[DetectorId] = @DetectorId
								  OR COALESCE (@DetectorId, '''') = '''')
								 AND (PL.Id = @LogId
								  OR COALESCE (@logid, '''') = '''')
								 AND ( LicensePlate.[LicensePlate] = @LicensePlate
									  OR COALESCE (@LicensePlate, '''') = '''')
								 AND ( LicensePlate.[FirstPart] = @FirstLicensePLatePart
									  OR COALESCE ( @FirstLicensePLatePart, '''') = '''')
								 AND ( LicensePlate.[SecondPart] = @SecondLicensePLatePart
									  OR COALESCE ( @SecondLicensePLatePart, '''') = '''')
								 AND ( LicensePlate.[ThirdPart] = @ThirdLicensePLatePart
									  OR COALESCE ( @ThirdLicensePLatePart, '''') = '''')
								 AND ( LicensePlate.[FourthPart] = @FourthLicensePLatePart
										  OR COALESCE ( @FourthLicensePLatePart, '''') = '''')
								 AND (isnull(@fromDate, '''') = ''''
									  OR CONVERT (DATE, PL.[LogDateTime]) >= CONVERT (DATE, @FromDate))
								 AND (isnull(@todate, '''') = ''''
									  OR CONVERT (DATE, PL.[LogDateTime]) <= CONVERT (DATE, @ToDate))
								AND (isnull(@MinPrecision, 0) = 0
									 OR PL.[DetectionPrecision] >= @MinPrecision)
								 AND (isnull(@MaxPrecision, 0) = 0
									  OR PL.[DetectionPrecision] <= @MaxPrecision)
								AND (isnull(@InOrOut, 0) = 0
									 OR PL.[InOrOut] = @InOrOut)
								 AND (Pl.SuccessTransfer = ISNULL(@SuccessTransfer, 1)
									  OR @SuccessTransfer IS NULL) ' + @Where + '
						/*ORDER BY [LogDateTime] DESC*/ ' + @Order
			
			SET @ParmDefinition = N'@DetectorId int, @LogId int, @LicensePlate nvarchar(20),@FirstLicensePLatePart nvarchar(10), @SecondLicensePLatePart nvarchar(10), @ThirdLicensePLatePart nvarchar(10), @FourthLicensePLatePart nvarchar(10),  @MinPrecision int ,@MaxPrecision int, @InOrOut int, @FromDate datetime,@ToDate datetime
										,@SuccessTransfer bit,@total int, @Message AS NVARCHAR (200), @Validate int, @Code nvarchar(5)';
            EXECUTE sp_executesql @DataSqlQuery, @ParmDefinition, @DetectorId, @LogId = @LogId, @LicensePlate = @LicensePlate, @FirstLicensePLatePart = @FirstLicensePLatePart, @SecondLicensePLatePart = @SecondLicensePLatePart, @ThirdLicensePLatePart = @ThirdLicensePLatePart, @FourthLicensePLatePart = @FourthLicensePLatePart, @MinPrecision = @MinPrecision, @MaxPrecision = @MaxPrecision, @InOrOut = @InOrOut, @FromDate = @FromDate, @ToDate = @ToDate
			, @SuccessTransfer = @SuccessTransfer, @total = @total, @Message = @Message, @Validate = @Validate, @Code = @Code;
            
			END
	END
END