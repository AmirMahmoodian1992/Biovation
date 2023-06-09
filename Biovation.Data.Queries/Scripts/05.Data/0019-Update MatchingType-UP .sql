CREATE TABLE #MatchingTypeTempTable(
ID int,
DeviceId int,
EventId int,
UserId int,
LogDate datetime,
Ticks bigint,
Subevent int,
TNAEvent int,
InOutMode int,
MatchingType int,
[Image] nvarchar(200),
SuccessTransfer bit,
CreateAt datetime)

INSERT INTO #MatchingTypeTempTable (ID,DeviceId,LogDate,EventId,UserId,Ticks,SubEvent,TNAEvent,InOutMode,MatchingType,[Image],SuccessTransfer,CreateAt) 
SELECT lg.Id,lg.DeviceId,lg.[DateTime],16003,lg.UserId,lg.Ticks,lg.SubEvent,lg.TNAEvent,lg.InOutMode,
									 CASE WHEN [MatchingType] = 3  THEN '19001'
                                          WHEN [MatchingType] = 1  THEN '19002' 
										  WHEN [MatchingType] = 2  THEN '19004' 
										  WHEN [MatchingType] = 0  THEN '19001' 
										  WHEN [MatchingType] = 15 THEN '19001' 
										  END,lg.[Image],lg.SuccessTransfer,lg.CreateAt
FROM dbo.[Log] lg
WHERE [EventId] NOT IN ('16001', '16002', '16003', '16004') AND [MatchingType] IN (1, 2, 3, 15, 0) AND UserId > 0

INSERT INTO #MatchingTypeTempTable (ID,DeviceId,LogDate,EventId,UserId,Ticks,SubEvent,TNAEvent,InOutMode,MatchingType,[Image],SuccessTransfer,CreateAt) 
SELECT lg.Id,lg.DeviceId,lg.[DateTime],lg.EventId,lg.UserId,lg.Ticks,lg.SubEvent,lg.TNAEvent,lg.InOutMode,
									 CASE WHEN [MatchingType] = 3  THEN '19001'
                                          WHEN [MatchingType] = 1  THEN '19002' 
										  WHEN [MatchingType] = 2  THEN '19004' 
										  WHEN [MatchingType] = 0  THEN '19001' 
										  WHEN [MatchingType] = 15 THEN '19001' 
										  END,lg.[Image],lg.SuccessTransfer,lg.CreateAt
FROM dbo.[Log] lg
WHERE [MatchingType] IN (1, 2, 3, 15, 0) AND [EventId] IN (16003, 16004);

INSERT INTO dbo.[Log] (DeviceId,[DateTime],EventId,UserId,Ticks,SubEvent,TNAEvent,InOutMode,MatchingType,[Image],SuccessTransfer,CreateAt) 
SELECT DISTINCT DeviceId,LogDate,EventId,UserId,Ticks,SubEvent,TNAEvent,InOutMode,MatchingType,[Image],SuccessTransfer,CreateAt  FROM #MatchingTypeTempTable as temp
WHERE NOT EXISTS (SELECT Id,[DateTime],Ticks, EventId,DeviceId,UserId,SubEvent,TNAEvent FROM dbo.[Log] lg
                  WHERE lg.Ticks = temp.Ticks AND lg.EventId = temp.EventId and lg.UserId = temp.UserId and lg.DeviceId = temp.DeviceId);

DELETE FROM dbo.[Log]
WHERE ([MatchingType] IN (1, 2, 3, 15, 0) AND [EventId] IN (16003, 16004)) OR [EventId] IN ('19001', '19002', '19004');



--IF EXISTS (SELECT MatchingType FROM dbo.[Log] WHERE  [MatchingType]= 3)
--    UPDATE dbo.[Log] SET [MatchingType] = '19001' WHERE [MatchingType]= 3

--IF EXISTS (SELECT MatchingType FROM dbo.[Log] WHERE  [MatchingType]= 1)
--    UPDATE dbo.[Log] SET [MatchingType] = '19002' WHERE [MatchingType]= 1

--IF EXISTS (SELECT MatchingType FROM dbo.[Log] WHERE  [MatchingType]= 2)
--    UPDATE dbo.[Log] SET [MatchingType] = '19004' WHERE [MatchingType]= 2

--IF EXISTS (SELECT MatchingType FROM dbo.[Log] WHERE  [MatchingType]= 15)
--    UPDATE dbo.[Log] SET [MatchingType] = '19001' WHERE [MatchingType]= 15

--IF EXISTS (SELECT MatchingType FROM dbo.[Log] WHERE  [MatchingType]= 0)
--    UPDATE dbo.[Log] SET [MatchingType] = '19002' WHERE [MatchingType]= 0