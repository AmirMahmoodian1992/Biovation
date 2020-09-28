

SET IDENTITY_INSERT [dbo].[TimeZone] ON 

IF NOT EXISTS (SELECT * FROM [dbo].[TimeZone] WHERE [Id] = 1)
	INSERT [dbo].[TimeZone] ([Id], [Name], [IsDefault]) VALUES (1, N'همیشه', 1)

IF NOT EXISTS (SELECT * FROM [dbo].[TimeZone] WHERE [Id] = 2)
	INSERT [dbo].[TimeZone] ([Id], [Name], [IsDefault]) VALUES (2, N'هیچ وقت', 1)

SET IDENTITY_INSERT [dbo].[TimeZone] OFF