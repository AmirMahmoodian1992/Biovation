CREATE PROCEDURE [RLY].[SelectSheduling]
@Id INT, @StartTime DATETIME, @EndTime DATETIME, @Mode int, @PageNumber INT=0, @PageSize INT=0
AS
DECLARE @HasPaging AS BIT;
DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (5) = N'200';
BEGIN
    SET @HasPaging = CASE WHEN @PageSize = 0
                               AND @PageNumber = 0 THEN 0 ELSE 1 END;
    IF @HasPaging = 1
        BEGIN
			SELECT	[Sh].[Id],
					[Sh].[StartTime],
					[Sh].[EndTime],
					[Sh].[Mode],
				    (@PageNumber - 1) * @PageSize AS [from],
                     @PageNumber AS PageNumber,
                     @PageSize AS PageSize,
                     count(*) OVER () AS [Count],
                     @Message AS e_Message,
                     @Code AS e_Code,
                     @Validate AS e_Validate
			FROM [Rly].[Sheduling] AS Sh
			WHERE   ([Sh].[Id] = @Id
						  OR ISNULL(@Id,'') = '')
					 AND ([Sh].[StartTime] = @StartTime
						  OR ISNULL(@StartTime,0) = 0)
   				     AND ([Sh].[EndTime] = @EndTime
						  OR ISNULL(@EndTime,0) = 0)
					 AND ([Sh].[Mode] = @Mode
						  OR ISNULL(@Mode,0) = 0)
			ORDER BY [Sh].Id
            OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
		END
		ELSE
			BEGIN
				SELECT [Sh].[Id],
					[Sh].[StartTime],
					[Sh].[EndTime],
					[Sh].[Mode],
				   1 AS [from],
				   1 AS PageNumber,
				   count(*) OVER () AS PageSize,
				   count(*) OVER () AS [Count],
                   @Message AS e_Message,
                   @Code AS e_Code,
                   @Validate AS e_Validate
				FROM [Rly].[Sheduling] AS Sh
				WHERE   ([Sh].[Id] = @Id
						  OR ISNULL(@Id,'') = '')
					 AND ([Sh].[StartTime] = @StartTime
						  OR ISNULL(@StartTime,0) = 0)
   				     AND ([Sh].[EndTime] = @EndTime
						  OR ISNULL(@EndTime,0) = 0)
					 AND ([Sh].[Mode] = @Mode
						  OR ISNULL(@Mode,0) = 0)
			END
	 
END