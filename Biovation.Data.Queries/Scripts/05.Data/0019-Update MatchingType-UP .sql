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
SELECT lg.Id,lg.DeviceId,lg.[DateTime],case when [MatchingType] = 3 then '19001'
                                          when [MatchingType] = 1 then '19002' 
										  when [MatchingType] = 2 then '19004' 
										  when [MatchingType] = 0 then '19001' 
										  when [MatchingType] = 15 then '19001' 
										  end,lg.UserId,lg.Ticks,lg.SubEvent,lg.TNAEvent,lg.InOutMode,lg.MatchingType,lg.[Image],lg.SuccessTransfer,lg.CreateAt
FROM dbo.[Log] lg
WHERE [MatchingType] in (1, 2, 3, 15, 0);

INSERT INTO dbo.[Log] (DeviceId,[DateTime],EventId,UserId,Ticks,SubEvent,TNAEvent,InOutMode,MatchingType,[Image],SuccessTransfer,CreateAt) 
SELECT DeviceId,LogDate,EventId,UserId,Ticks,SubEvent,TNAEvent,InOutMode,MatchingType,[Image],SuccessTransfer,CreateAt  FROM #MatchingTypeTempTable as temp
WHERE NOT EXISTS (SELECT Id,[DateTime],Ticks, EventId,DeviceId,UserId,SubEvent,TNAEvent FROM dbo.[Log] lg
                  WHERE lg.Ticks = temp.Ticks AND lg.EventId = temp.EventId and lg.UserId = temp.UserId and lg.DeviceId = temp.DeviceId);

DELETE FROM dbo.[Log]
WHERE [MatchingType] in (1, 2, 3, 15, 0);



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