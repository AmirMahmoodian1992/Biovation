CREATE TABLE #EventIdTempTable(
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

INSERT INTO #EventIdTempTable (ID,DeviceId,LogDate,EventId,UserId,Ticks,SubEvent,TNAEvent,InOutMode,MatchingType,[Image],SuccessTransfer,CreateAt) 
SELECT lg.Id,lg.DeviceId,lg.[DateTime],case when EventId = 88 then 16001
                                          when EventId = 87 then 16002 
										  when EventId = 55 then 16003 
										  when EventId = 56 then 16004 
										  when EventId = 44 then 16005 
										  when EventId = 45 then 16006
										  when EventId = 12 then 16007
										  when EventId = 13 then 16008
										  when EventId = 23 then 16009
										  when EventId = 61 then 16010
										  end,lg.UserId,lg.Ticks,lg.SubEvent,lg.TNAEvent,lg.InOutMode,lg.MatchingType,lg.[Image],lg.SuccessTransfer,lg.CreateAt
FROM dbo.[Log] lg
WHERE [EventId] in (88, 87,55,56,44,45,12,13,23,61)

INSERT INTO dbo.[Log] (DeviceId,[DateTime],EventId,UserId,Ticks,SubEvent,TNAEvent,InOutMode,MatchingType,[Image],SuccessTransfer,CreateAt) 
SELECT DeviceId,LogDate,EventId,UserId,Ticks,SubEvent,TNAEvent,InOutMode,MatchingType,[Image],SuccessTransfer,CreateAt  FROM #EventIdTempTable as temp
WHERE NOT EXISTS (SELECT Id,[DateTime],Ticks, EventId,DeviceId,UserId,SubEvent,TNAEvent FROM dbo.[Log] lg
                  WHERE lg.Ticks = temp.Ticks AND lg.EventId = temp.EventId and lg.UserId = temp.UserId and lg.DeviceId = temp.DeviceId);

DELETE FROM dbo.[Log]
WHERE [EventId] in (88, 87,55,56,44,45,12,13,23,61);







--IF EXISTS (SELECT EventId FROM dbo.[Log] WHERE  [EventId]= 88)
--    UPDATE dbo.[Log] SET [EventId] = '16001' WHERE [EventId]= 88

--IF EXISTS (SELECT EventId FROM dbo.[Log] WHERE  [EventId]= 87)
--   UPDATE dbo.[Log] SET [EventId] = '16002' WHERE [EventId]= 87

--IF EXISTS (SELECT EventId FROM dbo.[Log] WHERE  [EventId]= 55)
--     UPDATE dbo.[Log] SET [EventId] = '16003' WHERE [EventId]= 55

--IF EXISTS (SELECT EventId FROM dbo.[Log] WHERE  [EventId]= 56)
--   UPDATE dbo.[Log] SET [EventId] = '16004' WHERE [EventId]= 56

--IF EXISTS (SELECT EventId FROM dbo.[Log] WHERE  [EventId]= 44)
--      UPDATE dbo.[Log] SET [EventId] = '16005' WHERE [EventId]= 44

--IF EXISTS (SELECT EventId FROM dbo.[Log] WHERE  [EventId]= 45)
--    UPDATE dbo.[Log] SET [EventId] = '16006' WHERE [EventId]= 45

--IF EXISTS (SELECT EventId FROM dbo.[Log] WHERE  [EventId]= 12)
--    UPDATE dbo.[Log] SET [EventId] = '16007' WHERE [EventId]= 12

--IF EXISTS (SELECT EventId FROM dbo.[Log] WHERE  [EventId]= 13)
--   UPDATE dbo.[Log] SET [EventId] = '16008' WHERE [EventId]= 13

--IF EXISTS (SELECT EventId FROM dbo.[Log] WHERE  [EventId]= 23)
--   UPDATE dbo.[Log] SET [EventId] = '16009' WHERE [EventId]= 23

--IF EXISTS (SELECT EventId FROM dbo.[Log] WHERE  [EventId]= 61)
--  UPDATE dbo.[Log] SET [EventId] = '16010' WHERE [EventId]= 61