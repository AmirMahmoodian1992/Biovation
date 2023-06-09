
SET IDENTITY_INSERT [dbo].[TimeZoneDetail] ON 

IF NOT EXISTS (SELECT [Id] FROM [dbo].[TimeZoneDetail] WHERE [Id] = 1)
	INSERT [dbo].[TimeZoneDetail] ([Id], [TimeZoneId], [DayNumber], [FromTime], [ToTime]) VALUES (1, 1, 1, CAST(N'00:00:00' AS Time), CAST(N'23:59:59' AS Time))

IF NOT EXISTS (SELECT [Id] FROM [dbo].[TimeZoneDetail] WHERE [Id] = 2)
	INSERT [dbo].[TimeZoneDetail] ([Id], [TimeZoneId], [DayNumber], [FromTime], [ToTime]) VALUES (2, 1, 2, CAST(N'00:00:00' AS Time), CAST(N'23:59:59' AS Time))

IF NOT EXISTS (SELECT [Id] FROM [dbo].[TimeZoneDetail] WHERE [Id] = 3)
	INSERT [dbo].[TimeZoneDetail] ([Id], [TimeZoneId], [DayNumber], [FromTime], [ToTime]) VALUES (3, 1, 3, CAST(N'00:00:00' AS Time), CAST(N'23:59:59' AS Time))

IF NOT EXISTS (SELECT [Id] FROM [dbo].[TimeZoneDetail] WHERE [Id] = 4)
	INSERT [dbo].[TimeZoneDetail] ([Id], [TimeZoneId], [DayNumber], [FromTime], [ToTime]) VALUES (4, 1, 4, CAST(N'00:00:00' AS Time), CAST(N'23:59:59' AS Time))

IF NOT EXISTS (SELECT [Id] FROM [dbo].[TimeZoneDetail] WHERE [Id] = 5)
	INSERT [dbo].[TimeZoneDetail] ([Id], [TimeZoneId], [DayNumber], [FromTime], [ToTime]) VALUES (5, 1, 5, CAST(N'00:00:00' AS Time), CAST(N'23:59:59' AS Time))

IF NOT EXISTS (SELECT [Id] FROM [dbo].[TimeZoneDetail] WHERE [Id] = 6)
	INSERT [dbo].[TimeZoneDetail] ([Id], [TimeZoneId], [DayNumber], [FromTime], [ToTime]) VALUES (6, 1, 6, CAST(N'00:00:00' AS Time), CAST(N'23:59:59' AS Time))

IF NOT EXISTS (SELECT [Id] FROM [dbo].[TimeZoneDetail] WHERE [Id] = 7)
	INSERT [dbo].[TimeZoneDetail] ([Id], [TimeZoneId], [DayNumber], [FromTime], [ToTime]) VALUES (7, 1, 7, CAST(N'00:00:00' AS Time), CAST(N'23:59:59' AS Time))

IF NOT EXISTS (SELECT [Id] FROM [dbo].[TimeZoneDetail] WHERE [Id] = 8)
	INSERT [dbo].[TimeZoneDetail] ([Id], [TimeZoneId], [DayNumber], [FromTime], [ToTime]) VALUES (8, 2, 1, CAST(N'00:00:00' AS Time), CAST(N'00:01:00' AS Time))

IF NOT EXISTS (SELECT [Id] FROM [dbo].[TimeZoneDetail] WHERE [Id] = 9)
	INSERT [dbo].[TimeZoneDetail] ([Id], [TimeZoneId], [DayNumber], [FromTime], [ToTime]) VALUES (9, 2, 2, CAST(N'00:00:00' AS Time), CAST(N'00:01:00' AS Time))

IF NOT EXISTS (SELECT [Id] FROM [dbo].[TimeZoneDetail] WHERE [Id] = 10)
	INSERT [dbo].[TimeZoneDetail] ([Id], [TimeZoneId], [DayNumber], [FromTime], [ToTime]) VALUES (10, 2, 3, CAST(N'00:00:00' AS Time), CAST(N'00:01:00' AS Time))

IF NOT EXISTS (SELECT [Id] FROM [dbo].[TimeZoneDetail] WHERE [Id] = 11)
	INSERT [dbo].[TimeZoneDetail] ([Id], [TimeZoneId], [DayNumber], [FromTime], [ToTime]) VALUES (11, 2, 4, CAST(N'00:00:00' AS Time), CAST(N'00:01:00' AS Time))

IF NOT EXISTS (SELECT [Id] FROM [dbo].[TimeZoneDetail] WHERE [Id] = 12)
	INSERT [dbo].[TimeZoneDetail] ([Id], [TimeZoneId], [DayNumber], [FromTime], [ToTime]) VALUES (12, 2, 5, CAST(N'00:00:00' AS Time), CAST(N'00:01:00' AS Time))

IF NOT EXISTS (SELECT [Id] FROM [dbo].[TimeZoneDetail] WHERE [Id] = 13)
	INSERT [dbo].[TimeZoneDetail] ([Id], [TimeZoneId], [DayNumber], [FromTime], [ToTime]) VALUES (13, 2, 6, CAST(N'00:00:00' AS Time), CAST(N'00:01:00' AS Time))

IF NOT EXISTS (SELECT [Id] FROM [dbo].[TimeZoneDetail] WHERE [Id] = 14)
	INSERT [dbo].[TimeZoneDetail] ([Id], [TimeZoneId], [DayNumber], [FromTime], [ToTime]) VALUES (14, 2, 7, CAST(N'00:00:00' AS Time), CAST(N'00:01:00' AS Time))

SET IDENTITY_INSERT [dbo].[TimeZoneDetail] OFF