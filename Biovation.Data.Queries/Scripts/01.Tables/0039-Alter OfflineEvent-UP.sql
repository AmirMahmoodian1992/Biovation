IF COL_LENGTH('dbo.OfflineEvents', 'DeviceId') IS NOT NULL
BEGIN
	EXEC sp_RENAME 'OfflineEvents.DeviceId' , 'DeviceCode', 'COLUMN'
END

IF COL_LENGTH('dbo.OfflineEvents', 'Type') IS NULL
BEGIN
ALTER TABLE dbo.[OfflineEvents]
ADD
[Type] [int] NULL
END