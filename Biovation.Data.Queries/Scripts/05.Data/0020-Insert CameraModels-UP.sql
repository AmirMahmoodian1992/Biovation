
--------------------------------
-- Panasonic
--------------------------------



--------------------------------
-- MileSight
--------------------------------

IF NOT EXISTS (SELECT [Id] FROM [Rly].[CameraModel] WHERE [Id] = 2001)
	INSERT [Rly].[CameraModel] ([Id], [Name], [BrandCode], [ManufactureCode], [DefaultPortNumber], [DefaultUserName], [DefaultPassword], [Description], [RegisterDate]) VALUES (2001, N'Milesight', '22002', 0, 0,'username','password', 'Milesight', GETDATE())
