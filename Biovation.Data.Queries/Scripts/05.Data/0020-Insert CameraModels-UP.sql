IF NOT EXISTS (SELECT [Id] FROM [Rly].[CameraModel] WHERE [Id] = 1000)
	INSERT [Rly].[CameraModel] ([Id], [Name], [BrandCode], [ManufactureCode], [DefaultPortNumber], [DefaultUserName], [DefaultPassword], [Description], [RegisterDate]) VALUES (1000, N'متفرقه', '22000', 0, 0,'','', 'Other', GETDATE())


--------------------------------
-- Panasonic
--------------------------------

IF NOT EXISTS (SELECT [Id] FROM [Rly].[CameraModel] WHERE [Id] = 1001)
	INSERT [Rly].[CameraModel] ([Id], [Name], [BrandCode], [ManufactureCode], [DefaultPortNumber], [DefaultUserName], [DefaultPassword], [Description], [RegisterDate]) VALUES (1001, N'Panasonic', '22001', 0, 123,'admin','12345', 'Other', GETDATE())


--------------------------------
-- MileSight
--------------------------------

IF NOT EXISTS (SELECT [Id] FROM [Rly].[CameraModel] WHERE [Id] = 2001)
	INSERT [Rly].[CameraModel] ([Id], [Name], [BrandCode], [ManufactureCode], [DefaultPortNumber], [DefaultUserName], [DefaultPassword], [Description], [RegisterDate]) VALUES (2001, N'Milesight', '22002', 0, 0,'admin','password', 'Milesight', GETDATE())
