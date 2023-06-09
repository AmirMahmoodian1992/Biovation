Create PROCEDURE [dbo].[SelectCameraPlateDetectionLogs]
@FirstLicensePLatePart NVARCHAR (10)=NULL, @SecondLicensePLatePart NVARCHAR (10)=NULL, @ThirdLicensePLatePart NVARCHAR (10)=NULL, @FourthLicensePLatePart NVARCHAR (10)=NULL, @LogId INT=0, @LicensePlate NVARCHAR (20)=NULL, @DetectorId INT=NULL, @SuccessTransfer BIT=NULL, @FromDate DATETIME=NULL, @ToDate DATETIME=NULL, @MinPrecision TINYINT=0, @MaxPrecision TINYINT=0, @InOrOut TINYINT=0, @adminUserId INT=0, @WithPic BIT=0, @PageNumber INT=0, @PageSize INT=0, @Where NVARCHAR (MAX)='', @Order NVARCHAR (MAX)=''
AS
DECLARE @HasPaging AS BIT;
DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (5) = N'200';
BEGIN
    DECLARE @total AS INT;
    SET @HasPaging = CASE WHEN @PageSize = 0
                               AND @PageNumber = 0 THEN 0 ELSE 1 END;
    DECLARE @CountSqlQuery AS NVARCHAR (MAX);
    DECLARE @DataSqlQuery AS NVARCHAR (MAX);
    DECLARE @ParmDefinition AS NVARCHAR (500);
    SET @where = REPLACE(@where, 'DetectorId', 'DetectorId');
    SET @where = REPLACE(@where, 'True', '1');
    SET @where = REPLACE(@where, 'False', '0');
    SET @where = REPLACE(@where, 'LogDateTime', 'LogDateTime');
    SET @Order = REPLACE(@order, 'DetectorId', 'DetectorId');
    SET @Order = REPLACE(@order, 'LicensePlate', 'LicensePlate.LicensePlate');
    SET @Order = REPLACE(@order, 'LogDateTime', 'LogDateTime');
    SET @CountSqlQuery = '
	SELECT @total =
(SELECT COUNT(*)
  FROM [dbo].[PlateDetectionLog]  AS PL
           LEFT OUTER JOIN
                   [dbo].[Lookup] AS [LogEventLookUp]
                   ON PL.[EventId]  = [LogEventLookUp].[Code]
				   LEFT OUTER JOIN
				   [Rly].[Camera] AS C
				   ON PL.[DetectorId]  = [C].[Id]
				   LEFT OUTER JOIN
				   [dbo].[LicensePlate] AS [LicensePlate]
				   ON PL.LicensePlateId  = [LicensePlate].[EntityId]

		     	WHERE    (PL.[DetectorId] = @DetectorId
						  OR COALESCE (@DetectorId, 0) = 0)
						 AND (PL.Id = @LogId
						  OR COALESCE (@logid, 0) = 0)
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
						 ' + @Where + ')'
    SET @ParmDefinition = N'@DetectorId int, @LogId int, @LicensePlate nvarchar(20), @FirstLicensePLatePart nvarchar(10), @SecondLicensePLatePart nvarchar(10), @ThirdLicensePLatePart nvarchar(10), @FourthLicensePLatePart nvarchar(10), @MinPrecision int ,@MaxPrecision int, @InOrOut int, @FromDate datetime,@ToDate datetime,@SuccessTransfer bit,@total int OUTPUT';
    EXECUTE sp_executesql @CountSqlQuery, @ParmDefinition, @DetectorId, @LogId = @LogId, @LicensePlate = @LicensePlate, @FirstLicensePLatePart = @FirstLicensePLatePart, @SecondLicensePLatePart = @SecondLicensePLatePart, @ThirdLicensePLatePart = @ThirdLicensePLatePart, @FourthLicensePLatePart = @FourthLicensePLatePart, @MinPrecision = @MinPrecision, @MaxPrecision = @MaxPrecision, @InOrOut = @InOrOut, @FromDate = @FromDate, @ToDate = @ToDate, @SuccessTransfer = @SuccessTransfer, @total = @total OUTPUT;
    IF @Order = ''
        SET @Order = ' ORDER BY [LogDateTime] DESC';
    IF @HasPaging = 1
        BEGIN
            IF (@WithPic = 1)
                BEGIN
                    SET @Order = REPLACE(@order, 'LogDateTime', 'Data_LogDateTime');
                    SET @Order = REPLACE(@order, 'DetectorId', 'Data_DetectorId');
                    SET @Order = REPLACE(@order, 'LicensePlate', 'Data_LicensePlate_LicensePlateNumber');
                    SET @DataSqlQuery = '
					SELECT 
				   @total as Total
				  ,PL.[Id] AS Data_Id
				  ,[DetectorId] AS Data_DetectorId
				  ,[C].[Name] AS Data_DeviceName
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
							   [Rly].[Camera] AS C
								ON PL.[DetectorId]  = [C].[Id]
							   LEFT OUTER JOIN
							   [dbo].[PlateDetectionPictureLog] AS [LogPic]
							   ON PL.[Id]  = [LogPic].[LogId]
							   LEFT OUTER JOIN
							   [dbo].[LicensePlate] AS [LicensePlate]
							   ON [LicensePlate].[EntityId] =  PL.LicensePlateId

		     				WHERE    (PL.[DetectorId] = @DetectorId
									  OR COALESCE (@DetectorId, 0) = 0)
									 AND (PL.Id = @LogId
									  OR COALESCE (@logid, 0) = 0)
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
							OFFSET (@PageNumber-1)*@PageSize ROWS
							FETCH NEXT @PageSize ROWS ONLY
							/*ORDER BY [LogDateTime] DESC*/ ' + @Order
                    SET @ParmDefinition = N'@DetectorId int, @LogId int, @LicensePlate nvarchar(20), @FirstLicensePLatePart nvarchar(10), @SecondLicensePLatePart nvarchar(10), @ThirdLicensePLatePart nvarchar(10), @FourthLicensePLatePart nvarchar(10), @MinPrecision int ,@MaxPrecision int, @InOrOut int, @FromDate datetime,@ToDate datetime
										,@SuccessTransfer bit,@total int, @PageNumber int, @PageSize int, @Message AS NVARCHAR (200), @Validate int, @Code nvarchar(5)';
                    EXECUTE sp_executesql @DataSqlQuery, @ParmDefinition, @DetectorId, @LogId = @LogId, @LicensePlate = @LicensePlate, @FirstLicensePLatePart = @FirstLicensePLatePart, @SecondLicensePLatePart = @SecondLicensePLatePart, @ThirdLicensePLatePart = @ThirdLicensePLatePart, @FourthLicensePLatePart = @FourthLicensePLatePart, @MinPrecision = @MinPrecision, @MaxPrecision = @MaxPrecision, @InOrOut = @InOrOut, @FromDate = @FromDate, @ToDate = @ToDate, @SuccessTransfer = @SuccessTransfer, @total = @total, @PageNumber = @PageNumber, @PageSize = @PageSize, @Message = @Message, @Validate = @Validate, @Code = @Code;
                END
            ELSE
                BEGIN
                    SET @Order = REPLACE(@order, 'LogDateTime', 'Data_LogDateTime');
                    SET @Order = REPLACE(@order, 'DetectorId', 'Data_DetectorId');
                    SET @Order = REPLACE(@order, 'LicensePlate', 'Data_LicensePlate_LicensePlateNumber');
                    SET @DataSqlQuery = '
					SELECT  
				   @total as Total
				  ,PL.[Id] AS Data_Id
				  ,[DetectorId] AS Data_DetectorId
				  ,[C].[Name] AS Data_DeviceName
				  ,[LicensePlate].[EntityId] AS Data_LicensePlate_EntityId
				  ,[LicensePlate].[LicensePlate] AS Data_LicensePlate_LicensePlateNumber
				  ,[EventId] AS Data_EventLog_Code
				  ,[LogDateTime] AS Data_LogDateTime
				  ,[DetectionPrecision]  AS Data_DetectionPrecision
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
							   [Rly].[Camera] AS C
								ON PL.[DetectorId]  = [C].[Id]
							   LEFT OUTER JOIN
							   [dbo].[LicensePlate] AS [LicensePlate]
							   ON PL.LicensePlateId  = [LicensePlate].[EntityId]

		     				WHERE    (PL.[DetectorId] = @DetectorId
									  OR COALESCE (@DetectorId, 0) = 0)
									 AND (PL.Id = @LogId
									  OR COALESCE (@logid, 0) = 0)
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
										  OR @SuccessTransfer IS NULL)
								

									  ' + @Where + '
					
							/*ORDER BY [LogDateTime] DESC*/ ' + @Order + ' 
							OFFSET (@PageNumber-1)*@PageSize ROWS
							FETCH NEXT @PageSize ROWS ONLY';
                    SET @ParmDefinition = N'@DetectorId int, @LogId int, @LicensePlate nvarchar(20),@FirstLicensePLatePart nvarchar(10), @SecondLicensePLatePart nvarchar(10), @ThirdLicensePLatePart nvarchar(10), @FourthLicensePLatePart nvarchar(10),  @MinPrecision int ,@MaxPrecision int, @InOrOut int, @FromDate datetime,@ToDate datetime
										,@SuccessTransfer bit,@total int, @PageNumber int, @PageSize int, @Message AS NVARCHAR (200), @Validate int, @Code nvarchar(5)';
                    EXECUTE sp_executesql @DataSqlQuery, @ParmDefinition, @DetectorId, @LogId = @LogId, @LicensePlate = @LicensePlate, @FirstLicensePLatePart = @FirstLicensePLatePart, @SecondLicensePLatePart = @SecondLicensePLatePart, @ThirdLicensePLatePart = @ThirdLicensePLatePart, @FourthLicensePLatePart = @FourthLicensePLatePart, @MinPrecision = @MinPrecision, @MaxPrecision = @MaxPrecision, @InOrOut = @InOrOut, @FromDate = @FromDate, @ToDate = @ToDate, @SuccessTransfer = @SuccessTransfer, @total = @total, @PageNumber = @PageNumber, @PageSize = @PageSize, @Message = @Message, @Validate = @Validate, @Code = @Code;
                END
        END
    ELSE
        BEGIN
            IF (@WithPic = 1)
                BEGIN
                    SET @DataSqlQuery =  'SELECT @total AS Total
			  ,[PL].[Id] AS Data_Id
			  ,[DetectorId] AS Data_DetectorId
			  ,[C].[Name] AS Data_DeviceName
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
			   ,1 AS [from],
				   1 AS PageNumber,
				   count(*) OVER () AS PageSize,
				   count(*) OVER () AS [Count],
                   @Message AS e_Message,
                   @Code AS e_Code,
                   @Validate AS e_Validate
		  FROM [dbo].[PlateDetectionLog]  AS PL
				   LEFT OUTER JOIN
						   [dbo].[Lookup] AS [LogEventLookUp]
						   ON PL.[EventId]  = [LogEventLookUp].[Code]
						   LEFT OUTER JOIN
							[Rly].[Camera] AS C
							ON PL.[DetectorId]  = [C].[Id]
							LEFT OUTER JOIN
							[dbo].[PlateDetectionPictureLog] AS [LogPic]
							ON PL.[Id]  = [LogPic].[LogId]
						   LEFT OUTER JOIN
						   [dbo].[LicensePlate] AS [LicensePlate]
						   ON [LicensePlate].[EntityId] =  PL.LicensePlateId

		     			WHERE    (PL.[DetectorId] = @DetectorId
								  OR COALESCE (@DetectorId, 0) = 0)
								 AND (PL.Id = @LogId
								  OR COALESCE (@logid, 0) = 0)
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
									OR @SuccessTransfer IS NULL)
								  ' + @Where + '
						/*ORDER BY [LogDateTime] DESC*/ ' + @Order;
                    SET @ParmDefinition = N'@DetectorId int, @LogId int, @LicensePlate nvarchar(20),@FirstLicensePLatePart nvarchar(10), @SecondLicensePLatePart nvarchar(10), @ThirdLicensePLatePart nvarchar(10), @FourthLicensePLatePart nvarchar(10),  @MinPrecision int ,@MaxPrecision int, @InOrOut int, @FromDate datetime,@ToDate datetime
										,@SuccessTransfer bit,@total int, @Message AS NVARCHAR (200), @Validate int, @Code nvarchar(5)';
                    EXECUTE sp_executesql @DataSqlQuery, @ParmDefinition, @DetectorId, @LogId = @LogId, @LicensePlate = @LicensePlate, @FirstLicensePLatePart = @FirstLicensePLatePart, @SecondLicensePLatePart = @SecondLicensePLatePart, @ThirdLicensePLatePart = @ThirdLicensePLatePart, @FourthLicensePLatePart = @FourthLicensePLatePart, @MinPrecision = @MinPrecision, @MaxPrecision = @MaxPrecision, @InOrOut = @InOrOut, @FromDate = @FromDate, @ToDate = @ToDate, @SuccessTransfer = @SuccessTransfer, @total = @total, @Message = @Message, @Validate = @Validate, @Code = @Code;
                END
            ELSE
                BEGIN
                    SET @DataSqlQuery = 'SELECT @total as Total
			   ,[PL].[Id] AS Data_Id
			  ,[DetectorId] AS Data_DetectorId
			  ,[C].[Name] AS Data_DeviceName
			  ,[LicensePlate].[EntityId] AS Data_LicensePlate_EntityId
			  ,[LicensePlate].[LicensePlate] AS Data_LicensePlate_LicensePlateNumber
			  ,[EventId] AS Data_EventLog_Code
			  ,[LogDateTime] AS Data_LogDateTime
			  ,[DetectionPrecision] AS Data_DetectionPrecision
			  ,[SuccessTransfer] AS Data_SuccessTransfer
			  ,[InOrOut] AS Data_InOrOut
			  ,[LogEventLookUp].[Name] AS Data_EventLog_Name
			  ,[LogEventLookUp].[OrderIndex] AS Data_EventLog_OrderIndex
			  ,[LogEventLookUp].[Description] AS Data_EventLog_Description
			   ,1 AS [from],
				   1 AS PageNumber,
				   count(*) OVER () AS PageSize,
				   count(*) OVER () AS [Count],
                   @Message AS e_Message,
                   @Code AS e_Code,
                   @Validate AS e_Validate
		  FROM [dbo].[PlateDetectionLog]  AS PL
				   LEFT OUTER JOIN
						   [dbo].[Lookup] AS [LogEventLookUp]
						   ON PL.[EventId]  = [LogEventLookUp].[Code]
						   LEFT OUTER JOIN
							[Rly].[Camera] AS C
							ON PL.[DetectorId]  = [C].[Id]
						   LEFT OUTER JOIN
						   [dbo].[LicensePlate] AS [LicensePlate]
						   ON PL.LicensePlateId  = [LicensePlate].[EntityId]

		     			WHERE    (PL.[DetectorId] = @DetectorId
								  OR COALESCE (@DetectorId, 0) = 0)
								 AND (PL.Id = @LogId
								  OR COALESCE (@logid, 0) = 0)
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
										  OR @SuccessTransfer IS NULL)
								  ' + @Where  + '
						/*ORDER BY [LogDateTime] DESC*/ ' + @Order;
                    SET @ParmDefinition = N'@DetectorId int, @LogId int, @LicensePlate nvarchar(20),@FirstLicensePLatePart nvarchar(10), @SecondLicensePLatePart nvarchar(10), @ThirdLicensePLatePart nvarchar(10), @FourthLicensePLatePart nvarchar(10),  @MinPrecision int ,@MaxPrecision int, @InOrOut int, @FromDate datetime,@ToDate datetime
										,@SuccessTransfer bit,@total int, @Message AS NVARCHAR (200), @Validate int, @Code nvarchar(5)';
                    EXECUTE sp_executesql @DataSqlQuery, @ParmDefinition, @DetectorId, @LogId = @LogId, @LicensePlate = @LicensePlate, @FirstLicensePLatePart = @FirstLicensePLatePart, @SecondLicensePLatePart = @SecondLicensePLatePart, @ThirdLicensePLatePart = @ThirdLicensePLatePart, @FourthLicensePLatePart = @FourthLicensePLatePart, @MinPrecision = @MinPrecision, @MaxPrecision = @MaxPrecision, @InOrOut = @InOrOut, @FromDate = @FromDate, @ToDate = @ToDate, @SuccessTransfer = @SuccessTransfer, @total = @total, @Message = @Message, @Validate = @Validate, @Code = @Code;
                END
        END
END