
IF COL_LENGTH('dbo.Log', 'SuccessTransfer') IS NULL
BEGIN
    ALTER TABLE dbo.[Log]
ADD [SuccessTransfer] bit NULL

END

