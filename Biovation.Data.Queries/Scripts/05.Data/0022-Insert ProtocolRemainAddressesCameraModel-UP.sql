IF NOT EXISTS (SELECT [Id] FROM [Rly].[ProtocolRemainAddressesCameraModel] WHERE [Id] = 2001)
	INSERT [Rly].[ProtocolRemainAddressesCameraModel] ([ProtocolRemainAddressesId],[CameraModelId]) VALUES (1001,2001)
