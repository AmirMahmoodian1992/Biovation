IF COL_LENGTH('dbo.Log', 'Image') IS NULL
BEGIN
ALTER TABLE dbo.[Log]
ADD
[Image] nvarchar(2000)
END