IF NOT EXISTS (SELECT [Id] FROM [Rly].[RelayHubModel] WHERE [Id] = 1001)
	INSERT [Rly].[RelayHubModel] (	[Id], [Name], [ManufactureCode], [BrandId],	[DefaultCapacity], [DefaultPortNumber],	[Description]) VALUES (1001, 'Behsan ETH-04', 1, N'24001', 4, 23, N'ماژول کنترل رله تحت شبکه بهسان (ETH-04)')
