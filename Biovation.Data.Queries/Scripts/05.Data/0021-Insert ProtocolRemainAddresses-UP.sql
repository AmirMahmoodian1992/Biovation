--------------------------------
-- MileSight
--------------------------------

IF NOT EXISTS (SELECT [Id] FROM [Rly].[ProtocolRemainAddresses] WHERE [Id] = 1001)
	INSERT [Rly].[ProtocolRemainAddresses] ([Id], [ProtocolCode], [RemainAddress], [OrderIndex]) VALUES (1001, 20001, N'/main', 1)
ELSE
	UPDATE [Rly].[ProtocolRemainAddresses] SET [ProtocolCode] = 20001, [RemainAddress] = N'/main' WHERE [Id] = 1001

IF NOT EXISTS (SELECT [Id] FROM [Rly].[ProtocolRemainAddresses] WHERE [Id] = 1002)
	INSERT [Rly].[ProtocolRemainAddresses] ([Id], [ProtocolCode], [RemainAddress], [OrderIndex]) VALUES (1002, 20002, '/ipcam/mjpeg.cgi', 1)
ELSE
	UPDATE [Rly].[ProtocolRemainAddresses] SET [ProtocolCode] = 20002, [RemainAddress] = N'/ipcam/mjpeg.cgi' WHERE [Id] = 1002

--------------------------------
-- Panasonic
--------------------------------

IF NOT EXISTS (SELECT [Id] FROM [Rly].[ProtocolRemainAddresses] WHERE [Id] = 2001)
	INSERT [Rly].[ProtocolRemainAddresses] ([Id], [ProtocolCode], [RemainAddress], [OrderIndex]) VALUES (2001, 20001, N'/mediainput/h264', 1)
ELSE
	UPDATE [Rly].[ProtocolRemainAddresses] SET [ProtocolCode] = 20001, [RemainAddress] = N'/mediainput/h264' WHERE [Id] = 2001


IF NOT EXISTS (SELECT [Id] FROM [Rly].[ProtocolRemainAddresses] WHERE [Id] = 2002)
	INSERT [Rly].[ProtocolRemainAddresses] ([Id], [ProtocolCode], [RemainAddress], [OrderIndex]) VALUES (2002, 20002, N'/cgi-bin/mjpeg', 1)
ELSE
	UPDATE [Rly].[ProtocolRemainAddresses] SET [ProtocolCode] = 20002, [RemainAddress] = N'/mediainput/h264' WHERE [Id] = 2002
