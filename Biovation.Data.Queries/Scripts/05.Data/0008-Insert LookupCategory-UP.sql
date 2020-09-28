
IF NOT EXISTS (SELECT * FROM [dbo].[LookupCategory] WHERE [Id] = 1)
	INSERT INTO [dbo].[LookupCategory] ([Id], [Name], [Prefix], [Description]) VALUES (1, N'TaskStatus', N'1000', NULL)

IF NOT EXISTS (SELECT * FROM [dbo].[LookupCategory] WHERE [Id] = 2)
	INSERT INTO [dbo].[LookupCategory] ([Id], [Name], [Prefix], [Description]) VALUES (2, N'TaskTypes', N'1100', NULL)

IF NOT EXISTS (SELECT * FROM [dbo].[LookupCategory] WHERE [Id] = 3)
	INSERT INTO [dbo].[LookupCategory] ([Id], [Name], [Prefix], [Description]) VALUES (3, N'TaskItemTypes', N'1200', NULL)

IF NOT EXISTS (SELECT * FROM [dbo].[LookupCategory] WHERE [Id] = 4)
	INSERT INTO [dbo].[LookupCategory] ([Id], [Name], [Prefix], [Description]) VALUES (4, N'TaskPriority', N'1300', NULL)

IF NOT EXISTS (SELECT * FROM [dbo].[LookupCategory] WHERE [Id] = 5)
	INSERT INTO [dbo].[LookupCategory] ([Id], [Name], [Prefix], [Description]) VALUES (5, N'FingerIndexName', N'1400', NULL)
ELSE
	UPDATE [dbo].[LookupCategory] SET [Name] = N'FingerIndexName' WHERE [Id] = 5

IF NOT EXISTS (SELECT * FROM [dbo].[LookupCategory] WHERE [Id] = 6)
	INSERT INTO [dbo].[LookupCategory] ([Id], [Name], [Prefix], [Description]) VALUES (6, N'DeviceBrand', N'1500', NULL)

IF NOT EXISTS (SELECT * FROM [dbo].[LookupCategory] WHERE [Id] = 7)
	INSERT INTO [dbo].[LookupCategory] ([Id], [Name], [Prefix], [Description]) VALUES (7, N'LogEvent', N'1600', NULL)

IF NOT EXISTS (SELECT * FROM [dbo].[LookupCategory] WHERE [Id] = 8)
	INSERT INTO [dbo].[LookupCategory] ([Id], [Name], [Prefix], [Description]) VALUES (8, N'LogSubEvent', N'1700', NULL)

IF NOT EXISTS (SELECT * FROM [dbo].[LookupCategory] WHERE [Id] = 9)
	INSERT INTO [dbo].[LookupCategory] ([Id], [Name], [Prefix], [Description]) VALUES (9, N'FingerTemplateType', N'1800', NULL)

IF NOT EXISTS (SELECT * FROM [dbo].[LookupCategory] WHERE [Id] = 10)
	INSERT INTO [dbo].[LookupCategory] ([Id], [Name], [Prefix], [Description]) VALUES (10, N'FaceTemplateType', N'1810', NULL)

IF NOT EXISTS (SELECT * FROM [dbo].[LookupCategory] WHERE [Id] = 11)
	INSERT INTO [dbo].[LookupCategory] ([Id], [Name], [Prefix], [Description]) VALUES (11, N'MatchingType', N'1900', NULL)