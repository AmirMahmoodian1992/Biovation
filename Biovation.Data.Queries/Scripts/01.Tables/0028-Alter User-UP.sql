IF COL_LENGTH('dbo.User', 'EntityId') IS NULL
BEGIN
ALTER TABLE dbo.[User]
ADD
[EntityId] int
END
IF COL_LENGTH('dbo.User', 'Email') IS NULL
BEGIN
ALTER TABLE dbo.[User]
ADD
[Email] nvarchar(100) NULL
END
IF COL_LENGTH('dbo.User', 'TelNumber') IS NULL
BEGIN
ALTER TABLE dbo.[User]
ADD

[TelNumber] nvarchar(50) NULL


END
IF COL_LENGTH('dbo.User', 'Type') IS NULL
BEGIN
ALTER TABLE dbo.[User]
ADD
[Type] int NULL

END
IF COL_LENGTH('dbo.User', 'IsMasterAdmin') IS NULL
BEGIN
ALTER TABLE dbo.[User]
ADD
[Type] int NULL

END
