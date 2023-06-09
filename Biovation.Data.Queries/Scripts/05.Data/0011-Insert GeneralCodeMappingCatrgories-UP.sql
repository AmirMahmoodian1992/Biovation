
IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMappingCategory] WHERE [Id] = 1)
	INSERT INTO [dbo].[GenericCodeMappingCategory] ([Id], [Name], [Description]) VALUES (1, N'LogEventCode',  N'کد رویداد')
ELSE
	UPDATE [dbo].[GenericCodeMappingCategory] SET [Name] = N'LogEventCode',[Description]=N'کد رویداد' WHERE [Id] = 1


IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMappingCategory] WHERE [Id] = 2)
	INSERT INTO [dbo].[GenericCodeMappingCategory] ([Id], [Name], [Description]) VALUES (2, N'LogSubEventCode', N'کد تکمیلی رویداد')
ELSE
	UPDATE [dbo].[GenericCodeMappingCategory] SET [Name] = N'LogSubEventCode',[Description]= N'کد تکمیلی رویداد' WHERE [Id] = 2


IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMappingCategory] WHERE [Id] = 3)
	INSERT INTO [dbo].[GenericCodeMappingCategory] ([Id], [Name], [Description]) VALUES (3, N'AuthenticationTypeCode', N'نوع شناسایی')
ELSE
	UPDATE [dbo].[GenericCodeMappingCategory] SET [Name] = N'AuthenticationTypeCode',[Description]=N'نوع شناسایی' WHERE [Id] = 3


IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMappingCategory] WHERE [Id] = 4)
	INSERT INTO [dbo].[GenericCodeMappingCategory] ([Id], [Name], [Description]) VALUES (4, N'DeviceModelCode', N'مدل دستگاه')
ELSE
	UPDATE [dbo].[GenericCodeMappingCategory] SET [Name] = N'DeviceModelCode',[Description]=N'مدل دستگاه' WHERE [Id] = 4


IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMappingCategory] WHERE [Id] = 5)
	INSERT INTO [dbo].[GenericCodeMappingCategory] ([Id], [Name], [Description]) VALUES (5, N'DeviceFunctionTypeCode', N'نوع کاربری دستگاه')
ELSE
	UPDATE [dbo].[GenericCodeMappingCategory] SET [Name] = N'DeviceFunctionTypeCode',[Description]=N'نوع کاربری دستگاه' WHERE [Id] = 5


IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMappingCategory] WHERE [Id] = 6)
	INSERT INTO [dbo].[GenericCodeMappingCategory] ([Id], [Name], [Description]) VALUES (6, N'DeviceConnectionTypeCode', N'نوع ارتباط دستگاه')
ELSE
	UPDATE [dbo].[GenericCodeMappingCategory] SET [Name] = N'DeviceConnectionTypeCode',[Description]=N'نوع ارتباط دستگاه' WHERE [Id] = 6


IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMappingCategory] WHERE [Id] = 7)
	INSERT INTO [dbo].[GenericCodeMappingCategory] ([Id], [Name], [Description]) VALUES (7, N'UserAdminLevelCode', N'سطح دسترسی ادمین ')
ELSE
	UPDATE [dbo].[GenericCodeMappingCategory] SET [Name] = N'UserAdminLevelCode',[Description]=N'سطح دسترسی ادمین ' WHERE [Id] = 7


IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMappingCategory] WHERE [Id] = 8)
	INSERT INTO [dbo].[GenericCodeMappingCategory] ([Id], [Name], [Description]) VALUES (8, N'CardTypeCode', N'نوع کارت')
ELSE
	UPDATE [dbo].[GenericCodeMappingCategory] SET [Name] = N'CardTypeCode',[Description]=N'نوع کارت' WHERE [Id] = 8


IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMappingCategory] WHERE [Id] = 9)
	INSERT INTO [dbo].[GenericCodeMappingCategory] ([Id], [Name], [Description]) VALUES (9, N'FingerTemplateTypeCode', N'استاندارد اثر انگشت')
ELSE
	UPDATE [dbo].[GenericCodeMappingCategory] SET [Name] = N'FingerTemplateTypeCode',[Description]=N'استاندارد اثر انگشت' WHERE [Id] = 9


IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMappingCategory] WHERE [Id] = 10)
	INSERT INTO [dbo].[GenericCodeMappingCategory] ([Id], [Name], [Description]) VALUES (10, N'FingerIndexCode', N'شماره اثر انگشت')
ELSE
	UPDATE [dbo].[GenericCodeMappingCategory] SET [Name] = N'FingerIndexCode',[Description]=N'شماره اثر انگشت' WHERE [Id] = 10


IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMappingCategory] WHERE [Id] = 11)
	INSERT INTO [dbo].[GenericCodeMappingCategory] ([Id], [Name], [Description]) VALUES (11, N'WeekDayNumberCode', N'شماره  روزهای هفته')
ELSE
	UPDATE [dbo].[GenericCodeMappingCategory] SET [Name] = N'WeekDayNumberCode',[Description]= N'شماره  روزهای هفته' WHERE [Id] = 11


IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMappingCategory] WHERE [Id] = 12)
	INSERT INTO [dbo].[GenericCodeMappingCategory] ([Id], [Name], [Description]) VALUES (12, N'ErrorCode', N'شماره خطا')
ELSE
	UPDATE [dbo].[GenericCodeMappingCategory] SET [Name] = N'ErrorCode',[Description]=N'شماره خطا' WHERE [Id] = 12


IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMappingCategory] WHERE [Id] = 13)
	INSERT INTO [dbo].[GenericCodeMappingCategory] ([Id], [Name], [Description]) VALUES (13, N'SecurityLevelCode', N'سطح امنیت ')
ELSE
	UPDATE [dbo].[GenericCodeMappingCategory] SET [Name] = N'SecurityLevelCode',[Description]= N'سطح امنیت ' WHERE [Id] = 13

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMappingCategory] WHERE [Id] = 14)
	INSERT INTO [dbo].[GenericCodeMappingCategory] ([Id], [Name], [Description]) VALUES (14, N'FaceTemplateTypeCode', N'استاندارد چهره')
ELSE
	UPDATE [dbo].[GenericCodeMappingCategory] SET [Name] = N'FaceTemplateTypeCode',[Description]=N'استاندارد چهره' WHERE [Id] = 14

IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMappingCategory] WHERE [Id] = 15)
	INSERT INTO [dbo].[GenericCodeMappingCategory] ([Id], [Name], [Description]) VALUES (15, N'MatchingTypeCode', N'نوع تشخیص ')
ELSE
	UPDATE [dbo].[GenericCodeMappingCategory] SET [Name] = N'MatchingTypeCode',[Description]=N'نوع تشخیص' WHERE [Id] = 15


IF NOT EXISTS (SELECT [Id] FROM [dbo].[GenericCodeMappingCategory] WHERE [Id] = 16)
	INSERT INTO [dbo].[GenericCodeMappingCategory] ([Id], [Name], [Description]) VALUES (16, N'IrisTemplateTypeCode', N'استاندارد عنبیه')
ELSE
	UPDATE [dbo].[GenericCodeMappingCategory] SET [Name] = N'IrisTemplateTypeCode',[Description]=N'استاندارد عنبیه' WHERE [Id] = 16
