CREATE PROCEDURE [dbo].[SelectTimeZones]
@id INT = 0,
@name nvarchar(250) = NULL,
@accessGroupId INT = 0,
@pageNumber INT = 0,
@pageSize INT = 0
AS
DECLARE @Message AS NVARCHAR (200) = N' درخواست با موفقیت انجام گرفت', @Validate AS INT = 1,  @Code AS NVARCHAR (15) = N'200';
BEGIN
	DECLARE  @HasPaging   BIT;

    SET @HasPaging = CASE WHEN @PageSize = 0 OR @PageNumber = 0 THEN 0 ELSE 1 END;
	 IF @HasPaging = 1
		 BEGIN
		 SELECT * FROM
            (SELECT DENSE_RANK() OVER (ORDER BY Data_Id) AS RowNumber, * FROM
                (SELECT [TZ].[Id] AS Data_Id
                  ,[TZ].[Name] AS Data_Name
	              ,[TZD].[Id] AS Data_Details_Id
                  ,[TZD].[TimeZoneId] AS Data_Details_TimeZoneId
                  ,[TZD].[DayNumber] AS Data_Details_DayNumber
                  ,[TZD].[FromTime] AS Data_Details_FromTime
                  ,[TZD].[ToTime] AS Data_Details_ToTime
                  ,@Message AS e_Message
                  ,@Validate AS e_Validate
                  ,@Code AS e_Code
				  ,@pageNumber AS PageNumber
				  ,@pageSize AS PageSize
                  ,(@PageNumber - 1) * @PageSize AS [From]
	            FROM [dbo].[TimeZone] AS TZ 
                LEFT OUTER JOIN [dbo].[TimeZoneDetail] AS TZD 
                ON TZD.[TimeZoneId] = TZ.[Id]
                LEFT OUTER JOIN [dbo].[AccessGroup] AS AG
                ON AG.[TimeZoneId] = TZ.[Id]
                WHERE  ([TZ].[Id] = @id OR ISNULL(@id, 0) = 0)
                    AND ([TZ].[Name] LIKE '%' + @name + '%' OR ISNULL(@name, '') = '')
                    AND ([AG].[Id] = @accessGroupId OR ISNULL(@accessGroupId, '') = '')) AS TimeZones) AS TimeZonesOrdered
			WHERE RowNumber > (@PageNumber - 1) * @PageSize
			AND RowNumber <= @PageNumber * @PageSize
        END
    ELSE
        BEGIN
            SELECT [TZ].[Id] AS Data_Id
                  ,[TZ].[Name] AS Data_Name
	              ,[TZD].[Id] AS Data_Details_Id
                  ,[TZD].[TimeZoneId] AS Data_Details_TimeZoneId
                  ,[TZD].[DayNumber] AS Data_Details_DayNumber
                  ,[TZD].[FromTime] AS Data_Details_FromTime
                  ,[TZD].[ToTime] AS Data_Details_ToTime
                  ,@Message AS e_Message
                  ,@Validate AS e_Validate
                  ,@Code AS e_Code
				  ,@pageNumber AS PageNumber
				  ,@pageSize AS PageSize
                  ,(@PageNumber - 1) * @PageSize AS [From]
	            FROM [dbo].[TimeZone] AS TZ 
                LEFT OUTER JOIN [dbo].[TimeZoneDetail] AS TZD 
                ON TZD.[TimeZoneId] = TZ.[Id]
                LEFT OUTER JOIN [dbo].[AccessGroup] AS AG
                ON AG.[TimeZoneId] = TZ.[Id]
                WHERE  ([TZ].[Id] = @id OR ISNULL(@id, 0) = 0)
                    AND ([TZ].[Name] LIKE '%' + @name + '%' OR ISNULL(@name, '') = '')
                    AND ([AG].[Id] = @accessGroupId OR ISNULL(@accessGroupId, '') = '')
        END
END
GO
