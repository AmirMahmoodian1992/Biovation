IF NOT EXISTS (SELECT [Id] FROM [Rly].[CameraModel] WHERE [Id] = 2001)
	INSERT [Rly].[CameraModel] ([ProtocolRemainAddressesId],[CameraModelId]) VALUES (1001,2001)
