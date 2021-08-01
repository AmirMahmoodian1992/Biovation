--------------------------------
-- MileSight
--------------------------------

IF NOT EXISTS (SELECT [Id] FROM [Rly].[ProtocolRemainAddresses] WHERE [Id] = 1001)
	INSERT [Rly].[ProtocolRemainAddresses] ([Id], [ProtocolCode], [RemainAddress], [OrderIndex]) VALUES (1001, 20002, N'/main', 1)
ELSE
	UPDATE [Rly].[ProtocolRemainAddresses] SET [ProtocolCode] = 20002, [RemainAddress] = N'/main'

IF NOT EXISTS (SELECT [Id] FROM [Rly].[ProtocolRemainAddresses] WHERE [Id] = 1002)
	INSERT [Rly].[ProtocolRemainAddresses] ([Id], [ProtocolCode], [RemainAddress], [OrderIndex]) VALUES (1002, 20001, '/ipcam/mjpeg.cgi', 1)


--------------------------------
-- Panasonic
--------------------------------

IF NOT EXISTS (SELECT [Id] FROM [Rly].[ProtocolRemainAddresses] WHERE [Id] = 2001)
	INSERT [Rly].[ProtocolRemainAddresses] ([Id], [ProtocolCode], [RemainAddress], [OrderIndex]) VALUES (2001, 20002, N'/mediainput/h264', 1)
ELSE
	UPDATE [Rly].[ProtocolRemainAddresses] SET [ProtocolCode] = 20002, [RemainAddress] = N'/mediainput/h264'


IF NOT EXISTS (SELECT [Id] FROM [Rly].[ProtocolRemainAddresses] WHERE [Id] = 2002)
	INSERT [Rly].[ProtocolRemainAddresses] ([Id], [ProtocolCode], [RemainAddress], [OrderIndex]) VALUES (2002, 20001, N'/cgi-bin/mjpeg', 1)
