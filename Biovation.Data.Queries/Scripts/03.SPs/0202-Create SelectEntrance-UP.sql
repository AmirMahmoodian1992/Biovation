CREATE PROCEDURE [dbo].[SelectEntrance]
@Id INT= NULL, @Code BIGINT = NULL, @Name NVARCHAR(50)= NULL, @SchedulingId INT= NULL, @CameraId INT= NULL, @PageNumber INT=0, @PageSize INT=0
AS
DECLARE @HasPaging AS BIT;
DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @ResultCode AS NVARCHAR (5) = N'200';
BEGIN
    SET @HasPaging = CASE WHEN @PageSize = 0
                               AND @PageNumber = 0 THEN 0 ELSE 1 END;
    IF @HasPaging = 1
        BEGIN
			SELECT [Ent].[Id] AS Data_Id,
				   [Ent].[Code] AS Data_Code,
				   [Ent].[Name] AS Data_Name,
				   [Ent].[Description] AS Data_Description,
				   [C].[Id] AS  Data_Cameras_Id,
				   [C].[Code] AS  Data_Cameras_Code,
				   [C].[Active] AS  Data_Cameras_Active,
				   [C].[ModelId] AS  Data_Cameras_ModelId,
				   [C].[Name] AS  Data_Cameras_Name,
				   [C].[Ip] AS  Data_Cameras_Ip,
				   [C].[Port] AS  Data_Cameras_Port,
				   [C].[MacAddress] AS  Data_Cameras_MacAddress,
				   [C].[RegisterDate] AS  Data_Cameras_RegisterDate,
				   [C].[HardwareVersion] AS  Data_Cameras_HardwareVersion,
				   [C].[BrandCode] AS  Data_Cameras_Brand_Code,
				   [C].[UserName] AS  Data_Cameras_UserName,
				   [C].[Password] AS  Data_Cameras_Password,
				   [C].[ConnectionUrl] AS  Data_Cameras_ConnectionUrl,
				   [C].[LiveStreamUrl] AS  Data_Cameras_LiveStreamUrl,
				   [C].[SerialNumber] AS Data_Cameras_SerialNumber,
				   [S].[Id] AS Data_Schedulings_Id,
				   [S].[StartTime] AS Data_Schedulings_StartTime,
				   [S].[EndTime] AS Data_Schedulings_EndTime,
				   [S].[Mode] AS Data_Schedulings_Mode,
				    (@PageNumber - 1) * @PageSize AS [from],
                     @PageNumber AS PageNumber,
                     @PageSize AS PageSize,
                     count(*) OVER () AS [Count],
                     @Message AS e_Message,
                     @ResultCode AS e_Code,
                     @Validate AS e_Validate
			FROM [Rly].[Entrance] AS Ent
				LEFT OUTER JOIN
				[Rly].[EntranceCamera] AS EC
				ON EC.EntranceId = Ent.[Id]
				LEFT OUTER JOIN
				[Rly].Camera AS C
				ON C.Id = EC.CameraId
				LEFT OUTER JOIN
				[Rly].[EntranceScheduling] AS ES
				ON ES.EntranceId = Ent.[Id]
				LEFT OUTER JOIN
				[Rly].Scheduling AS S
				ON S.Id = ES.SchedulingId
			WHERE   ([Ent].[Id] = @Id
						  OR ISNULL(@Id,'') = '')
					 AND ([Ent].[Name] = @Name
						  OR ISNULL(@Name,0) = 0)
   				     AND ([ES].[SchedulingId] = @SchedulingId
						  OR ISNULL(@SchedulingId,0) = 0)
					 AND ([EC].[CameraId] = @CameraId
						  OR ISNULL(@CameraId,0) = 0)
					 AND ([Ent].[Code] = @Code
						  OR ISNULL(@Code,0) = 0)
			ORDER BY [Ent].Id
            OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
		END
		ELSE
			BEGIN
				SELECT [Ent].[Id] AS Data_Id,
				   [Ent].[Code] AS Data_Code,
				   [Ent].[Name] AS Data_Name,
				   [Ent].[Description] AS Data_Description,
				   [C].[Id] AS  Data_Cameras_Id,
				   [C].[Code] AS  Data_Cameras_Code,
				   [C].[Active] AS  Data_Cameras_Active,
				   [C].[ModelId] AS  Data_Cameras_ModelId,
				   [C].[Name] AS  Data_Cameras_Name,
				   [C].[Ip] AS  Data_Cameras_Ip,
				   [C].[Port] AS  Data_Cameras_Port,
				   [C].[MacAddress] AS  Data_Cameras_MacAddress,
				   [C].[RegisterDate] AS  Data_Cameras_RegisterDate,
				   [C].[HardwareVersion] AS  Data_Cameras_HardwareVersion,
				   [C].[BrandCode] AS  Data_Cameras_Brand_Code,
				   [C].[UserName] AS  Data_Cameras_UserName,
				   [C].[Password] AS  Data_Cameras_Password,
				   [C].[ConnectionUrl] AS  Data_Cameras_ConnectionUrl,
				   [C].[LiveStreamUrl] AS  Data_Cameras_LiveStreamUrl,
				   [C].[SerialNumber] AS Data_Cameras_SerialNumber,
				   [S].[Id] AS Data_Schedulings_Id,
				   [S].[StartTime] AS Data_Schedulings_StartTime,
				   [S].[EndTime] AS Data_Schedulings_EndTime,
				   [S].[Mode] AS Data_Schedulings_Mode,
				   1 AS [from],
				   1 AS PageNumber,
				   count(*) OVER () AS PageSize,
				   count(*) OVER () AS [Count],
                   @Message AS e_Message,
                   @ResultCode AS e_Code,
                   @Validate AS e_Validate
				FROM [Rly].[Entrance] AS Ent
				LEFT OUTER JOIN
				[Rly].[EntranceCamera] AS EC
				ON EC.EntranceId = Ent.[Id]
				LEFT OUTER JOIN
				[Rly].Camera AS C
				ON C.Id = EC.CameraId
				LEFT OUTER JOIN
				[Rly].[EntranceScheduling] AS ES
				ON ES.EntranceId = Ent.[Id]
				LEFT OUTER JOIN
				[Rly].Scheduling AS S
				ON S.Id = ES.SchedulingId
			WHERE   ([Ent].[Id] = @Id
						  OR ISNULL(@Id,'') = '')
					 AND ([Ent].[Name] = @Name
						  OR ISNULL(@Name,0) = 0)
   				     AND ([ES].[SchedulingId] = @SchedulingId
						  OR ISNULL(@SchedulingId,0) = 0)
					 AND ([Ent].[Code] = @Code
						  OR ISNULL(@Code,0) = 0)
			END
	 
END