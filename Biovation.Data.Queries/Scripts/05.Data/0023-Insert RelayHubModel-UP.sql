IF NOT EXISTS (SELECT [ProtocolRemainAddressesId] FROM [Rly].[RelayHubModel] WHERE [Id] = 1000)
	INSERT [Rly].[RelayHubModel] ([Id],[Name], [ManufactureCode], [BrandId],[DefaultCapacity], [DefaultPortNumber], [Description]) VALUES (1000, N'Behsan_ETH04', 0, N'24001', 4 , 8081, N'ما')
