IF COL_LENGTH('dbo.User', 'RemainingCredit') IS NULL
BEGIN
ALTER TABLE dbo.[User]
ADD
[RemainingCredit] [real] NULL
END

IF COL_LENGTH('dbo.User', 'AllowedStockCount') IS NULL
BEGIN
ALTER TABLE dbo.[User]
ADD
[AllowedStockCount] [int] NULL
END