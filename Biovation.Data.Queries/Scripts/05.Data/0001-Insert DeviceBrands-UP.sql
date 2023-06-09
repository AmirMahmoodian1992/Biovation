
IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceBrand] WHERE [Name] = 'Virdi')
	INSERT [dbo].[DeviceBrand] ([Id], [Name], [Description]) VALUES ('15001', N'Virdi', N'Unicom Communication Co. ,Unis Serivces')
ELSE
	UPDATE [dbo].[DeviceBrand] SET [Id] = '15001' WHERE [Name] = N'Virdi'

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceBrand] WHERE [Name] = 'EOS')
	INSERT [dbo].[DeviceBrand] ([Id], [Name], [Description]) VALUES ('15002', N'EOS', N'Elm-o-Sanat Devices')
ELSE
	UPDATE [dbo].[DeviceBrand] SET [Id] = '15002' WHERE [Name] = N'EOS'

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceBrand] WHERE [Name] = 'Suprema')
	INSERT [dbo].[DeviceBrand] ([Id], [Name], [Description]) VALUES ('15003', N'Suprema', N'Suprema Co.')
ELSE
	UPDATE [dbo].[DeviceBrand] SET [Id] = '15003' WHERE [Name] = N'Suprema'

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceBrand] WHERE [Name] = 'ZK')
	INSERT [dbo].[DeviceBrand] ([Id], [Name], [Description]) VALUES ('15004', N'ZK', N'ZKTeco Co.')
ELSE
	UPDATE [dbo].[DeviceBrand] SET [Id] = '15004' WHERE [Name] = N'ZK'

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceBrand] WHERE [Name] = 'PW')
	INSERT [dbo].[DeviceBrand] ([Id], [Name], [Description]) VALUES ('15005', N'PW', N'Processing World Co.')
ELSE
	UPDATE [dbo].[DeviceBrand] SET [Id] = '15005' WHERE [Name] = N'PW'

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceBrand] WHERE [Name] = 'Maxa')
	INSERT [dbo].[DeviceBrand] ([Id], [Name], [Description]) VALUES ('15006', N'Maxa', N'Kasra Co. Restaurant Managment System')
ELSE IF EXISTS (SELECT [Id] FROM [dbo].[DeviceBrand] WHERE [Id] = '15006')
	UPDATE [dbo].[DeviceBrand] SET [Name] = N'Maxa', [Description] = N'Kasra Co. Restaurant Managment System' WHERE [Id] = '15006'
ELSE
	UPDATE [dbo].[DeviceBrand] SET [Id] = '15006' WHERE [Name] = N'Maxa'

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceBrand] WHERE [Name] = 'Shahab')
	INSERT [dbo].[DeviceBrand] ([Id], [Name], [Description]) VALUES ('15007', N'Shahab', N'Shahab license detector')
ELSE
	UPDATE [dbo].[DeviceBrand] SET [Id] = '15007' WHERE [Name] = N'Shahab'

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceBrand] WHERE [Name] = 'Paliz')
	INSERT [dbo].[DeviceBrand] ([Id], [Name], [Description]) VALUES ('15009', N'Paliz', N'PalizAfzar Co')
ELSE
	UPDATE [dbo].[DeviceBrand] SET [Id] = '15009' WHERE [Name] = N'Paliz'