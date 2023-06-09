
--------------------------------
-- Virdi
--------------------------------

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 1001)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description]) VALUES (1001, N'AC6000', '15001', 6000, 0, N'AC-6000')
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15001' WHERE [Id] = 1001

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 1002)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description]) VALUES (1002, N'AC2100', '15001', 2100, 0, N'AC-2100')
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15001' WHERE [Id] = 1002

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 1003)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description]) VALUES (1003, N'AC2200', '15001', 10, 0, N'AC-2200')
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15001' WHERE [Id] = 1003

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 1004)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description]) VALUES (1004, N'AC7000', '15001', 7000, 0, N'AC-7000')
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15001' WHERE [Id] = 1004

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 1005)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description]) VALUES (1005, N'U-Bio Pro', '15001', 0, 0, N'U-Bio Pro')
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15001' WHERE [Id] = 1005


--------------------------------
-- EOS
--------------------------------

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 2001)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description]) VALUES (2001, N'ST-Pro', '15002', 0, 0, 'Suprema Sensor')
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15002', [Description] = 'Suprema Sensor' WHERE [Id] = 2001

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 2002)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description]) VALUES (2002, N'ST-Pro +', '15002', 0, 0, 'Suprema Sensor')
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15002', [Description] = 'Suprema Sensor' WHERE [Id] = 2002

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 2003)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description], [DefaultPortNumber]) VALUES (2003, N'ST-Shine L', '15002', 0, 0, 'Suprema Sensor', 1001)
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15002' WHERE [Id] = 2003

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 2004)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description], [DefaultPortNumber]) VALUES (2004, N'ST-Shine M', '15002', 0, 0, 'ZK Sensor', 1001)
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15002', [DefaultPortNumber] = 1001 WHERE [Id] = 2004

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 2005)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description], [DefaultPortNumber]) VALUES (2005, N'ST-Face 110', '15002', 0, 0, 'Hanvon Sensor', 9922)
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15002' WHERE [Id] = 2005

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 2006)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description], [DefaultPortNumber]) VALUES (2006, N'ST-Face 120', '15002', 0, 0, 'ZK Sensor', 4370)
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15002' WHERE [Id] = 2006

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 2007)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description], [DefaultPortNumber]) VALUES (2007, N'ST-Face 130', '15002', 0, 0, 'ZK Sensor', 4370)
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15002' WHERE [Id] = 2007

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 2008)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description], [DefaultPortNumber]) VALUES (2008, N'ST-Face 160', '15002', 0, 0, 'ZK Sensor', 4370)
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15002' WHERE [Id] = 2008

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 2009)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description], [DefaultPortNumber]) VALUES (2009, N'ST-Face 710', '15002', 0, 0, 'Hanvon Sensor', 9922)
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15002' WHERE [Id] = 2009

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 2010)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description], [DefaultPortNumber]) VALUES (2010, N'ST-Eco 210', '15002', 0, 0, 'ZK Sensor', 4370)
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15002' WHERE [Id] = 2010

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 2011)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description], [DefaultPortNumber]) VALUES (2011, N'ST-P220', '15002', 0, 0, 'ZK Sensor', 4370)
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15002' WHERE [Id] = 2011

--------------------------------
-- Suprema V.1
--------------------------------

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 3001)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description]) VALUES (3001, N'Biomini', '15003', 3001, 0, N'نرم افزار شبیه ساز ساعت کسرا با استفاده از سنسور (همستر) های سوپریما')
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15003' WHERE [Id] = 3001

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 3002)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description]) VALUES (3002, N'BioLiteNet.V1', '15003', 2, 0, NULL)
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15003' WHERE [Id] = 3002

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 3003)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description]) VALUES (3003, N'BioStation', '15003', 0, 0, NULL)
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15003' WHERE [Id] = 3003

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 3004)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description]) VALUES (3004, N'FaceStation.V1', '15003', 10, 0, NULL)
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15003' WHERE [Id] = 3004

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 3005)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description]) VALUES (3005, N'BioStationT2', '15003', 6, 0, NULL)
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15003' WHERE [Id] = 3005


--------------------------------
-- ZkTeco
--------------------------------

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 4001)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description]) VALUES (4001, N'FaceG3', '15004', 0, 0, NULL)
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15004' WHERE [Id] = 4001

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 4002)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description]) VALUES (4002, N'UFace202', '15004', 0, 0, NULL)
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15004' WHERE [Id] = 4002

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 4003)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description]) VALUES (4003, N'UFace402', '15004', 0, 0, NULL)
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15004' WHERE [Id] = 4003

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 4004)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description]) VALUES (4004, N'UFace602', '15004', 0, 0, NULL)
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15004' WHERE [Id] = 4004

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 4005)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description]) VALUES (4005, N'IFace202', '15004', 0, 0, NULL)
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15004' WHERE [Id] = 4005

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 4006)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description]) VALUES (4006, N'BW', '15004', 0, 0, NULL)
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15004' WHERE [Id] = 4006

--------------------------------
-- PW
--------------------------------

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 5001)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description]) VALUES (5001, N'PW1100', '15005', 1, 0, N'دنیای پردازش 1100')
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15005' WHERE [Id] = 5001

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 5002)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description]) VALUES (5002, N'PW1200', '15005', 2, 0, N'دنیای پردازش 1200')
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15005' WHERE [Id] = 5002

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 5003)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description]) VALUES (5003, N'PW1400', '15005', 3, 0, N'دنیای پردازش 1400')
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15005' WHERE [Id] = 5003

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 5004)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description]) VALUES (5004, N'PW1410', '15005', 4, 0, N'دنیای پردازش 1410')
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15005' WHERE [Id] = 5004

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 5005)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description]) VALUES (5005, N'PW1500', '15005', 5, 0, N'دنیای پردازش 1500')
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15005' WHERE [Id] = 5005

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 5006)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description]) VALUES (5006, N'PW1510', '15005', 6, 0, N'دنیای پردازش 1510')
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15005' WHERE [Id] = 5006

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 5007)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description]) VALUES (5007, N'PW1520', '15005', 7, 0, N'دنیای پردازش 1520')
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15005' WHERE [Id] = 5007

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 5008)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description]) VALUES (5008, N'PW1600', '15005', 8, 0, N'دنیای پردازش 1600')
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15005' WHERE [Id] = 5008

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 5009)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description]) VALUES (5009, N'PW1650', '15005', 9, 0, N'دنیای پردازش 1650')
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15005' WHERE [Id] = 5009

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 5010)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description]) VALUES (5010, N'PW1680', '15005', 10, 0, N'دنیای پردازش 1680')
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15005' WHERE [Id] = 5010

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 5011)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description]) VALUES (5011, N'PW1700', '15005', 11, 0, N'دنیای پردازش 1700')
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15005' WHERE [Id] = 5011

--------------------------------
-- Simulators
--------------------------------

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 6001)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description]) VALUES (6001, N'MaxaClient', '15006', 1, 0, N'نرم افزار شبیه ساز دستگاه Maxa')
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15006' WHERE [Id] = 6001


--------------------------------
-- Plate Detectors
--------------------------------

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 7001)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description]) VALUES (7001, N'GenralCamera', '15007', 1, 0, N'دوربین عمومی برای پلاک خوان')
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15007' WHERE [Id] = 7001


--------------------------------
-- Paliz
--------------------------------

IF NOT EXISTS (SELECT [Id] FROM [dbo].[DeviceModel] WHERE [Id] = 8001)
	INSERT [dbo].[DeviceModel] ([Id], [Name], [BrandId], [ManufactureCode], [GetLogMethodType], [Description], [DefaultPortNumber]) VALUES (8001, N'Tiara', '15009', 0, 0, 'Virdi Sensor', 1883)
ELSE
	UPDATE [dbo].[DeviceModel] SET [BrandId] = '15009' WHERE [Id] = 8001
