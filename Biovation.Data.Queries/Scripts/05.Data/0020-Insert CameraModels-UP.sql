IF NOT EXISTS (SELECT [Id] FROM [Rly].[CameraModel] WHERE [Id] = 1000)
	INSERT [Rly].[CameraModel] ([Id], [Name], [BrandCode], [ManufactureCode], [DefaultPortNumber], [DefaultUserName], [DefaultPassword], [Description], [RegisterDate]) VALUES (1000, N'متفرقه', '22000', 0, 554,'','', N'Other', GETDATE())
ELSE
	UPDATE [Rly].[CameraModel] SET [DefaultPortNumber] = 554 WHERE [Id] = 1000

--------------------------------
-- Panasonic
--------------------------------

IF NOT EXISTS (SELECT [Id] FROM [Rly].[CameraModel] WHERE [Id] = 1001)
	INSERT [Rly].[CameraModel] ([Id], [Name], [BrandCode], [ManufactureCode], [DefaultPortNumber], [DefaultUserName], [DefaultPassword], [Description], [RegisterDate]) VALUES (1001, N'Panasonic', '22001', 0, 554, N'admin', N'12345', N'دوربین های پاناسونیک', GETDATE())
ELSE
	UPDATE [Rly].[CameraModel] SET [DefaultPortNumber] = 554, [DefaultUserName] = N'admin', [DefaultPassword] = N'12345', [Description] = N'دوربین های پاناسونیک' WHERE [Id] = 1001

--------------------------------
-- MileSight
--------------------------------

IF NOT EXISTS (SELECT [Id] FROM [Rly].[CameraModel] WHERE [Id] = 2001)
	INSERT [Rly].[CameraModel] ([Id], [Name], [BrandCode], [ManufactureCode], [DefaultPortNumber], [DefaultUserName], [DefaultPassword], [Description], [RegisterDate]) VALUES (2001, N'Milesight', '22002', 0, 554, N'admin', N'ms1234', N'دوربین های Milesight', GETDATE())
ELSE
	UPDATE [Rly].[CameraModel] SET [DefaultPortNumber] = 554, [DefaultUserName] = N'admin', [DefaultPassword] = N'ms1234', [Description] = N'دوربین های Milesight' WHERE [Id] = 2001
