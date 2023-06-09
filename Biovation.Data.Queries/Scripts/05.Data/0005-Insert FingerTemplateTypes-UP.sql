
IF NOT EXISTS (SELECT [Id] FROM [dbo].[FingerTemplateType] WHERE [Id] = 11)
	INSERT INTO [dbo].[FingerTemplateType] ([Id] ,[BrandId] ,[Type] ,[Description]) VALUES (11, 1, N'V400', N'Virdi 400 Byte template standard')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[FingerTemplateType] WHERE [Id] = 31)
	INSERT INTO [dbo].[FingerTemplateType] ([Id] ,[BrandId] ,[Type] ,[Description]) VALUES (31, 3, N'SU384', N'Suprema 384 Byte template standard')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[FingerTemplateType] WHERE [Id] = 10)
	INSERT INTO [dbo].[FingerTemplateType] ([Id] ,[BrandId] ,[Type] ,[Description]) VALUES (10, 4, N'VX10', N'ZKV10.0')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[FingerTemplateType] WHERE [Id] = 7)
	INSERT INTO [dbo].[FingerTemplateType] ([Id] ,[BrandId] ,[Type] ,[Description]) VALUES (7, 4, N'VX7', N'ZK Face VX7.0')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[FingerTemplateType] WHERE [Id] = 40)
	INSERT INTO [dbo].[FingerTemplateType] ([Id] ,[BrandId] ,[Type] ,[Description]) VALUES (40, 1, N'VFACE', N'FACE FOR VIRDI')
