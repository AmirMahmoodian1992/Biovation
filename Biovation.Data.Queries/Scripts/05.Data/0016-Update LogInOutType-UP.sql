
IF NOT EXISTS (SELECT Id FROM dbo.[Log] WHERE [InOutMode] IN (0, 1, 2))
	UPDATE dbo.[Log] SET [Log].[InOutMode] = [Device].DeviceTypeId FROM dbo.[Device]  WHERE [Device].Id = [Log].DeviceId