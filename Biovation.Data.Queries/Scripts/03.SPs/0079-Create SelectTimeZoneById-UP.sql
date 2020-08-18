
CREATE PROCEDURE [dbo].[SelectTimeZoneById]
	@Id int
AS
BEGIN
    SELECT [TZ].[Id]
      ,[TZ].[Name]
	  ,[TZD].[Id] AS Details_Id
      ,[TZD].[TimeZoneId] AS Details_TimeZoneId
      ,[TZD].[DayNumber] AS Details_DayNumber
      ,[TZD].[FromTime] AS Details_FromTime
      ,[TZD].[ToTime] AS Details_ToTime
	FROM [dbo].[TimeZone] AS TZ LEFT JOIN [dbo].[TimeZoneDetail] AS TZD ON TZD.[TimeZoneId] = TZ.[Id]
	WHERE TZ.[Id] = @Id
END
GO
