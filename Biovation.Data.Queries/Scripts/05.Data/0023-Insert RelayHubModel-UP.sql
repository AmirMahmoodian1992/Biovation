IF NOT EXISTS (SELECT [Id] FROM [Rly].[RelayHubModel] WHERE [Id] = 1001)
	INSERT [Rly].[ProtocolRemainAddressesCameraModel] (	[Id], [Name], [ManufactureCode], [BrandId],	[DefaultCapacity], [DefaultPortNumber],	[Description]) VALUES (1001, 'Behsan', 1, 24001, 4, 23, 'Behsan Ethernet Relay Controller (ETH-04)')
