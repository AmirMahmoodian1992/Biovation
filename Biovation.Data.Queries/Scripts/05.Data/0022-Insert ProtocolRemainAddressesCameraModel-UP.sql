IF NOT EXISTS (SELECT [ProtocolRemainAddressesId] FROM [Rly].[ProtocolRemainAddressesCameraModel] WHERE [ProtocolRemainAddressesId] = 1001 and [CameraModelId] = 2001)
	INSERT [Rly].[ProtocolRemainAddressesCameraModel] ([ProtocolRemainAddressesId],[CameraModelId]) VALUES (1001,2001)
