IF COL_LENGTH('dbo.Device', 'ServiceInstanceId') IS NULL
BEGIN
ALTER TABLE dbo.[Device]
ADD
[ServiceInstanceId] NVARCHAR(MAX) NULL
END
GO