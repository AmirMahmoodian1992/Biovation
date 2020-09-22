CREATE PROCEDURE [dbo].[SelectPlateDetectionLogs]
 @LogId INT=0,
 @LicensePlate nvarchar=NULL,
 @DetectorId INT =NULL,
 @SuccessTransfer BIT = NULL, 
 @FromDate DATETIME = NULL, 
 @ToDate DATETIME = NULL,
 @MinPrecision tinyint = 0,
 @MaxPrecision  tinyint = 0,
 @adminUserId INT=0,
 @WithPic bit =1,
 @PageNumber AS INT=0,
 @PageSize AS INT =0
AS
          DECLARE  @HasPaging   BIT;
		  DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS nvarchar(5) = N'200';
   
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
IF(@WithPic = 1)
			BEGIN
    SELECT PL.[Id] AS Data_Id
      ,[DetectorId] AS Data_DetectorId
	  ,[LicensePlate].[EntityId] As Data_LicensePlate_EntityId
      ,[LicensePlate].[LicensePlate] As Data_LicensePlate_LicensePlateNumber
      ,[EventId] AS Data_EventLog_Code
	  ,[LogDateTime] AS Data_LogDateTime
      ,[DetectionPrecision]  AS Data_DetectionPrecision
      ,[LogPic].[FullImage] AS Data_FullImage
      ,[LogPic].[PlateImage] AS Data_PlateImage
      ,[SuccessTransfer] AS Data_SuccessTransfer
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
						  OR COALESCE (@DetectorId, '') = '')
						 AND (PL.Id = @LogId
						  OR COALESCE (@logid, '') = '')
						 AND ( LicensePlate.[LicensePlate] = @LicensePlate
							  OR COALESCE ( @LicensePlate, '') = '')
						 AND (isnull(@fromDate, '') = ''
							  OR CONVERT (DATE, PL.[LogDateTime]) >= CONVERT (DATE, @FromDate))
						 AND (isnull(@todate, '') = ''
							  OR CONVERT (DATE, PL.[LogDateTime]) <= CONVERT (DATE, @ToDate))
						AND (isnull(@MinPrecision, 0) = 0
						     OR PL.[DetectionPrecision] >= @MinPrecision)
						 AND (isnull(@MaxPrecision, 0) = 0
							  OR PL.[DetectionPrecision] <= @MinPrecision)
						 AND (Pl.SuccessTransfer = ISNULL(@SuccessTransfer, 1)
							  OR @SuccessTransfer IS NULL)
				ORDER BY [LogDateTime] DESC
				OFFSET (@PageNumber-1)*@PageSize ROWS
                FETCH NEXT @PageSize ROWS ONLY
		END 	   
ELSE
		BEGIN 
		    SELECT        	 
	   PL.[Id] AS Data_Id
      ,[DetectorId] AS Data_DetectorId
	  ,[LicensePlate].[EntityId] As Data_LicensePlate_EntityId
      ,[LicensePlate].[LicensePlate] As Data_LicensePlate_LicensePlateNumber
      ,[EventId] AS Data_EventLog_Code
	  ,[LogDateTime] AS Data_LogDateTime
      ,[DetectionPrecision]  AS Data_DetectionPrecision
      ,[SuccessTransfer] AS Data_SuccessTransfer
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
						  OR COALESCE (@DetectorId, '') = '')
						 AND (PL.Id = @LogId
						  OR COALESCE (@logid, '') = '')
						 AND ( LicensePlate.[LicensePlate] = @LicensePlate
							  OR COALESCE (@LicensePlate, '') = '')
						 AND (isnull(@fromDate, '') = ''
							  OR CONVERT (DATE, PL.[LogDateTime]) >= CONVERT (DATE, @FromDate))
						 AND (isnull(@todate, '') = ''
							  OR CONVERT (DATE, PL.[LogDateTime]) <= CONVERT (DATE, @ToDate))
						AND (isnull(@MinPrecision, 0) = 0
						     OR PL.[DetectionPrecision] >= @MinPrecision)
						 AND (isnull(@MaxPrecision, 0) = 0
							  OR PL.[DetectionPrecision] <= @MaxPrecision)
						 AND (Pl.SuccessTransfer = ISNULL(@SuccessTransfer, 1)
							  OR @SuccessTransfer IS NULL)
				ORDER BY [LogDateTime] DESC
		        OFFSET (@PageNumber-1)*@PageSize ROWS
                FETCH NEXT @PageSize ROWS ONLY
	END
END
ELSE
BEGIN
IF(@WithPic = 1)
			BEGIN
    SELECT PL.[Id]
      ,[DetectorId]
	  ,[LicensePlate].[EntityId] As LicensePlate_EntityId
      ,[LicensePlate].[LicensePlate] As LicensePlate_LicensePlateNumber
      ,[EventId] AS EventLog_Code
	  ,[LogDateTime]
      ,[DetectionPrecision]
      ,[LogPic].[FullImage]
      ,[LogPic].[PlateImage]
      ,[SuccessTransfer]
	  ,[LogEventLookUp].[Name] AS EventLog_Name
	  ,[LogEventLookUp].[OrderIndex] AS EventLog_OrderIndex
	  ,[LogEventLookUp].[Description] AS EventLog_Description
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
						  OR COALESCE (@DetectorId, '') = '')
						 AND (PL.Id = @LogId
						  OR COALESCE (@logid, '') = '')
						 AND ( LicensePlate.[LicensePlate] = @LicensePlate
							  OR COALESCE ( @LicensePlate, '') = '')
						 AND (isnull(@fromDate, '') = ''
							  OR CONVERT (DATE, PL.[LogDateTime]) >= CONVERT (DATE, @FromDate))
						 AND (isnull(@todate, '') = ''
							  OR CONVERT (DATE, PL.[LogDateTime]) <= CONVERT (DATE, @ToDate))
						AND (isnull(@MinPrecision, 0) = 0
						     OR PL.[DetectionPrecision] >= @MinPrecision)
						 AND (isnull(@MaxPrecision, 0) = 0
							  OR PL.[DetectionPrecision] <= @MinPrecision)
						 AND (Pl.SuccessTransfer = ISNULL(@SuccessTransfer, 1)
							  OR @SuccessTransfer IS NULL)
				ORDER BY [LogDateTime] DESC
		END 	   
ELSE
		BEGIN 
		    SELECT PL.[Id]
      ,[DetectorId]
      ,[LicensePlate].[EntityId] As LicensePlate_EntityId
      ,[LicensePlate].[LicensePlate] As LicensePlate_LicensePlateNumber
      ,[LogDateTime]
      ,[DetectionPrecision]
      ,[SuccessTransfer]
	  ,[LogEventLookUp].[Name] AS EventLog_Name
	  ,[EventId] AS EventLog_Code
	  ,[LogEventLookUp].[OrderIndex] AS EventLog_OrderIndex
	  ,[LogEventLookUp].[Description] AS EventLog_Description
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
						  OR COALESCE (@DetectorId, '') = '')
						 AND (PL.Id = @LogId
						  OR COALESCE (@logid, '') = '')
						 AND ( LicensePlate.[LicensePlate] = @LicensePlate
							  OR COALESCE (@LicensePlate, '') = '')
						 AND (isnull(@fromDate, '') = ''
							  OR CONVERT (DATE, PL.[LogDateTime]) >= CONVERT (DATE, @FromDate))
						 AND (isnull(@todate, '') = ''
							  OR CONVERT (DATE, PL.[LogDateTime]) <= CONVERT (DATE, @ToDate))
						AND (isnull(@MinPrecision, 0) = 0
						     OR PL.[DetectionPrecision] >= @MinPrecision)
						 AND (isnull(@MaxPrecision, 0) = 0
							  OR PL.[DetectionPrecision] <= @MaxPrecision)
						 AND (Pl.SuccessTransfer = ISNULL(@SuccessTransfer, 1)
							  OR @SuccessTransfer IS NULL)
				ORDER BY [LogDateTime] DESC
	END
END
END