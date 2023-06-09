CREATE PROCEDURE [dbo].[SelectScheduling]
@Id INT = NULL, @StartTime TIME= NULL, @EndTime TIME= NULL, @Mode NVARCHAR(10)= NULL, @PageNumber INT=0, @PageSize INT=0
AS
DECLARE @HasPaging AS BIT;
DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (5) = N'200';
BEGIN
    SET @HasPaging = CASE WHEN @PageSize = 0
                               AND @PageNumber = 0 THEN 0 ELSE 1 END;
    IF @HasPaging = 1
        BEGIN
			SELECT	[Sh].[Id] AS Data_Id,
					[Sh].[StartTime] AS Data_StartTime,
					[Sh].[EndTime] AS Data_EndTime,
					[Sh].[Mode] AS Data_Mode,
					[L].[Name] AS  Data_Mode_Name,
                    [L].[OrderIndex] AS Data_Mode_OrderIndex,
                    [L].[Description] AS  Data_Mode_Description,
                    [L].[LookupCategoryId] AS  Data_Mode_Category_Id,
				    (@PageNumber - 1) * @PageSize AS [from],
                     @PageNumber AS PageNumber,
                     @PageSize AS PageSize,
                     count(*) OVER () AS [Count],
                     @Message AS e_Message,
                     @Code AS e_Code,
                     @Validate AS e_Validate
			FROM [Rly].[Scheduling] AS Sh
			LEFT JOIN
			[dbo].[Lookup] AS L
            ON [L].[Code] = [SH].[Mode]
			WHERE   ([Sh].[Id] = @Id
						  OR ISNULL(@Id,0) = 0)
					 AND (@StartTime IS NULL OR [Sh].[StartTime] >= @StartTime)
   				     AND (@EndTime IS NULL OR [Sh].[EndTime] <= @EndTime)
					 AND ([Sh].[Mode] = @Mode
						  OR ISNULL(@Mode,'') = '')
			ORDER BY [Sh].Id
            OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
		END
		ELSE
			BEGIN
				SELECT	[Sh].[Id] AS Data_Id,
					[Sh].[StartTime] AS Data_StartTime,
					[Sh].[EndTime] AS Data_EndTime,
					[Sh].[Mode] AS Data_Mode,
					[L].[Name] AS  Data_Mode_Name,
                    [L].[OrderIndex] AS Data_Mode_OrderIndex,
                    [L].[Description] AS  Data_Mode_Description,
                    [L].[LookupCategoryId] AS  Data_Mode_Category_Id,
				   1 AS [from],
				   1 AS PageNumber,
				   count(*) OVER () AS PageSize,
				   count(*) OVER () AS [Count],
                   @Message AS e_Message,
                   @Code AS e_Code,
                   @Validate AS e_Validate
				FROM [Rly].[Scheduling] AS Sh
			LEFT JOIN
			[dbo].[Lookup] AS L
            ON [L].[Code] = [SH].[Mode]
				WHERE   ([Sh].[Id] = @Id
						  OR ISNULL(@Id,0) = 0)
					 AND (@StartTime IS NULL OR [Sh].[StartTime] >= @StartTime)
   				     AND (@EndTime IS NULL OR [Sh].[EndTime] <= @EndTime)
					 AND ([Sh].[Mode] = @Mode
						  OR ISNULL(@Mode,'') = '')
			END
	 
END