IF NOT EXISTS (SELECT [ProtocolRemainAddressesId] FROM [Rly].[ProtocolRemainAddressesCameraModel] WHERE [ProtocolRemainAddressesId] = 1001 and [CameraModelId] = 2001)
	INSERT [Rly].[ProtocolRemainAddressesCameraModel] ([ProtocolRemainAddressesId],[CameraModelId]) VALUES (1001, 2001)


IF NOT EXISTS (SELECT [ProtocolRemainAddressesId] FROM [Rly].[ProtocolRemainAddressesCameraModel] WHERE [ProtocolRemainAddressesId] = 1002 and [CameraModelId] = 2001)
	INSERT [Rly].[ProtocolRemainAddressesCameraModel] ([ProtocolRemainAddressesId],[CameraModelId]) VALUES (1002, 2001)

