IF COL_LENGTH('dbo.DeviceModel', 'DefaultPortNumber') IS NULL
BEGIN
ALTER TABLE [dbo].[DeviceModel] ADD [DefaultPortNumber]INT NULL
END