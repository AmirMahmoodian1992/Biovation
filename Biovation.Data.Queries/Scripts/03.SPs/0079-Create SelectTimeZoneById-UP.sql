
CREATE PROCEDURE [dbo].[SelectTimeZoneById]
	@Id int
AS
DECLARE @Message AS NVARCHAR (200) = N' درخواست با موفقیت انجام گرفت', @Validate AS INT = 1,  @Code AS NVARCHAR (15) = N'200';

BEGIN
    SELECT [TZ].[Id]
      ,[TZ].[Name]
	  ,[TZD].[Id] AS Details_Id
      ,[TZD].[TimeZoneId] AS Details_TimeZoneId
      ,[TZD].[DayNumber] AS Details_DayNumber
      ,[TZD].[FromTime] AS Details_FromTime
      ,[TZD].[ToTime] AS Details_ToTime
       ,@Message AS e_Message,
        @Validate AS e_Validate,
        @Code AS e_Code
	FROM [dbo].[TimeZone] AS TZ LEFT JOIN [dbo].[TimeZoneDetail] AS TZD ON TZD.[TimeZoneId] = TZ.[Id]
	WHERE TZ.[Id] = @Id
END
GO
