IF COL_LENGTH('dbo.Log', 'CreateAt') IS NULL
BEGIN
ALTER TABLE dbo.[Log]
ADD
[CreateAt] [datetime] NULL
END
