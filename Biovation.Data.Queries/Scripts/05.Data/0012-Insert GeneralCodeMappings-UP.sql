
--------------------------------
--Log  Event Code Mappings
--------------------------------

Delete From [dbo].[GenericCodeMapping] WHERE [CategoryId] = 1 AND [ManufactureCode] = '88' AND [GenericCode] = '16001' AND [BrandCode] = '15003'

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15003' AND [CategoryId] = 1 AND [ManufactureCode] = 211)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (1, '211', '16001', '15003', N'اتصال')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15003' AND [CategoryId] = 1 AND [ManufactureCode] = 212)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (1, '212', '16002', '15003', N'قطع اتصال')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15003' AND [CategoryId] = 1 AND [ManufactureCode] = 213)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (1, '213', '16001', '15003', N'اتصال')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15003' AND [CategoryId] = 1 AND [ManufactureCode] = 214)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (1, '214', '16002', '15003', N'قطع اتصال')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15003' AND [CategoryId] = 1 AND [ManufactureCode] = 23)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (1, '23', '16009', '15003', N'تعریف کاربر با موفقیت')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15003' AND [CategoryId] = 1 AND [ManufactureCode] = 55)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (1, '55', '16003', '15003', N'شناسایی با موفقی')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15003' AND [CategoryId] = 1 AND [ManufactureCode] = 56)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (1, '56', '16004', '15003', N'عدم شناسایی')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15003' AND [CategoryId] = 1 AND [ManufactureCode] = 61)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (1, '61', '16003', '15003', N'شناسایی با موفقیت (فقط با چهره)')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15003' AND [CategoryId] = 1 AND [ManufactureCode] = 194)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (1, '194', '16004', '15003', N'عدم اجازه عبور')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15003' AND [CategoryId] = 1 AND [ManufactureCode] = 243)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (1, '243', '16001', '15003', N'اتصال')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15003' AND [CategoryId] = 1 AND [ManufactureCode] = 244)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (1, '244', '16002', '15003', N'قطع اتصال')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15003' AND [CategoryId] = 1 AND [ManufactureCode] = 119)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (1, '119', '16004', '15003', N'کاربر منقضی شده')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15003' AND [CategoryId] = 1 AND [ManufactureCode] = 71)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (1, '71', '16006', '15003', N'حذف کاربر')


--------------------------------
--Log Sub Event Code Mappings
--------------------------------

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15001' AND [CategoryId] = 2 AND [ManufactureCode] = 3)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (2, '3', '17001', '15001', N'شناسایی عادی')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15001' AND [CategoryId] = 2 AND [ManufactureCode] = 1)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (2, '1', '17002', '15001', N'کلید F1')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15001' AND [CategoryId] = 2 AND [ManufactureCode] = 2)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (2, '2', '17003', '15001', N'کلید F2')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15001' AND [CategoryId] = 2 AND [ManufactureCode] = 4)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (2, '4', '17004', '15001', N'کلید F3')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15001' AND [CategoryId] = 2 AND [ManufactureCode] = 5)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (2, '5', '17005', '15001', N'کلید F4')


	
--------------------------------
--FingerTemplate Code Mappings
--------------------------------

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15001' AND [CategoryId] = 9 AND [ManufactureCode] = 400)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (9, '400', '18001', '15001', N'Virdi 400 Byte template standard')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15004' AND [CategoryId] = 9 AND [ManufactureCode] = 10)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (9, '10', '18002', '15004', N'ZKV10.0')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15003' AND [CategoryId] = 9 AND [ManufactureCode] = 31)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (9, '31', '18003', '15003', N'Suprema 384 Byte template standard')


--------------------------------
--FaceTemplate Code Mappings
--------------------------------

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15001' AND [CategoryId] = 14 AND [ManufactureCode] = 40)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (14, '40', '18101', '15001', N'VFACE')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15004' AND [CategoryId] = 14 AND [ManufactureCode] = 7)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (14, '7', '18102', '15004', N'ZK Face VX7.0')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15001' AND [CategoryId] = 14 AND [ManufactureCode] = '13')
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (14, '13', '18105', '15001', N'VWTFace UBIO PRO 2')

	--------------------------------
--Matching Type Code Mappings
--------------------------------

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15001' AND [CategoryId] = 15 AND [ManufactureCode] = 0)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (15, '0', '19002', '15001', N'FingerTemplate')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15004' AND [CategoryId] = 15 AND [ManufactureCode] = 1)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (15, '1', '19002', '15004', N'FingerTemplate')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15002' AND [CategoryId] = 15 AND [ManufactureCode] = 58)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (15, '58', '19002', '15002', N'FingerTemplate')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15004' AND [CategoryId] = 15 AND [ManufactureCode] = 4)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (15, '4', '19004', '15004', N'Card')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15004' AND [CategoryId] = 15 AND [ManufactureCode] = 15)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (15, '15', '19001', '15004', N'Face')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15001' AND [CategoryId] = 15 AND [ManufactureCode] = 5)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (15, '5', '19001', '15001', N'Face')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15001' AND [CategoryId] = 15 AND [ManufactureCode] = 3)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (15, '3', '19004', '15001', N'Card')



--------------------------------
--Finger Index Code Mappings
--------------------------------

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15009' AND [CategoryId] = 10 AND [ManufactureCode] = 1)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (10, '1', '14005', '15009', N'کوچک راست')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15009' AND [CategoryId] = 10 AND [ManufactureCode] = 2)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (10, '2', '14004', '15009', N'حلقه راست')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15009' AND [CategoryId] = 10 AND [ManufactureCode] = 3)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (10, '3', '14003', '15009', N'میانی راست')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15009' AND [CategoryId] = 10 AND [ManufactureCode] = 4)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (10, '4', '14002', '15009', N'اشاره راست')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15009' AND [CategoryId] = 10 AND [ManufactureCode] = 5)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (10, '5', '14001', '15009', N'شست راست')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15009' AND [CategoryId] = 10 AND [ManufactureCode] = 6)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (10, '6', '140010', '15009', N'شست چپ')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15009' AND [CategoryId] = 10 AND [ManufactureCode] = 7)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (10, '7', '14009', '15009', N'اشاره چپ')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15009' AND [CategoryId] = 10 AND [ManufactureCode] = 8)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (10, '8', '14008', '15009', N'میانی چپ')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15009' AND [CategoryId] = 10 AND [ManufactureCode] = 9)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (10, '9', '14007', '15009', N'حلقه چپ')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15009' AND [CategoryId] = 10 AND [ManufactureCode] = 10)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (10, '10', '14006', '15009', N'کوچک چپ')

--------------------------------
--Iris Template Type
--------------------------------

--------------------------------
--Finger Index Code Mappings
--------------------------------

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15009' AND [CategoryId] = 10 AND [ManufactureCode] = 1)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (10, '1', '14005', '15009', N'کوچک راست')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15009' AND [CategoryId] = 10 AND [ManufactureCode] = 2)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (10, '2', '14004', '15009', N'حلقه راست')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15009' AND [CategoryId] = 10 AND [ManufactureCode] = 3)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (10, '3', '14003', '15009', N'میانی راست')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15009' AND [CategoryId] = 10 AND [ManufactureCode] = 4)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (10, '4', '14002', '15009', N'اشاره راست')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15009' AND [CategoryId] = 10 AND [ManufactureCode] = 5)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (10, '5', '14001', '15009', N'شست راست')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15009' AND [CategoryId] = 10 AND [ManufactureCode] = 6)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (10, '6', '140010', '15009', N'شست چپ')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15009' AND [CategoryId] = 10 AND [ManufactureCode] = 7)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (10, '7', '14009', '15009', N'اشاره چپ')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15009' AND [CategoryId] = 10 AND [ManufactureCode] = 8)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (10, '8', '14008', '15009', N'میانی چپ')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15009' AND [CategoryId] = 10 AND [ManufactureCode] = 9)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (10, '9', '14007', '15009', N'حلقه چپ')

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15009' AND [CategoryId] = 10 AND [ManufactureCode] = 10)
	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (10, '10', '14006', '15009', N'کوچک چپ')



--for suprema
--IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15003' AND [CategoryId] = 1 AND [ManufactureCode] = 24)
--	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (1, '24', '19999', '15003', N'enroll fail')

--IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15003' AND [CategoryId] = 1 AND [ManufactureCode] = 25)
--	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (1, '25', '19999', '15003', N'fingerprint scan fail')

--IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15003' AND [CategoryId] = 1 AND [ManufactureCode] = 32) /[Id]we havn't any Generic Code Mapping Category for card errors[Id]/
--	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (1, '32', '19999', '15003', N'card enroll succeed')

--IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15003' AND [CategoryId] = 1 AND [ManufactureCode] = 33)
--	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (1, '33', '19999', '15003', N'card enroll failed')



--IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15003' AND [CategoryId] = 1 AND [ManufactureCode] = 22)
--	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (1, '22', '19999', '15003', N'enroll bad finger')

--IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15003' AND [CategoryId] = 1 AND [ManufactureCode] = 36)
--	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (1, '36', '19999', '15003', N'verify bad face')

--IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15003' AND [CategoryId] = 1 AND [ManufactureCode] = 38)
--	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (1, '38', '19999', '15003', N'verify bad finger')

--IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15003' AND [CategoryId] = 1 AND [ManufactureCode] = 39)
--	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (1, '39', '19999', '15003', N'verify success')

--IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMapping] WHERE [BrandCode] = '15003' AND [CategoryId] = 1 AND [ManufactureCode] = 40)
--	INSERT [dbo].[GenericCodeMapping] ([CategoryId], [ManufactureCode], [GenericCode], [BrandCode], [Description]) VALUES (1, '40', '19999', '15003', N'verify fail')





