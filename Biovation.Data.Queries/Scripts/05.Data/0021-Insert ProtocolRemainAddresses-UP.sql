IF NOT EXISTS (SELECT [Id] FROM [Rly].[ProtocolRemainAddresses] WHERE [Id] = 1001)
	INSERT [Rly].[ProtocolRemainAddresses] ([Id], [ProtocolCode], [RemainAddress], [OrderIndex]) VALUES (1001,2001,'/ipcam/mjpeg.cgi',1)
