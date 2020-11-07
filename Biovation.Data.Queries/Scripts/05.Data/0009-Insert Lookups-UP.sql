
--------------------------------
--Task Statuses
--------------------------------

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'10001' AND [LookupCategoryId] = 1)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'10001', 1, N'Queued', 1, N'در انتظار اجرا')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'10002' AND [LookupCategoryId] = 1)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'10002', 1, N'Successful', 2, N'انجام شده')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'10003' AND [LookupCategoryId] = 1)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'10003', 1, N'Failed', 3, N'مشکل در عملیات')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'10004' AND [LookupCategoryId] = 1)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'10004', 1, N'In Progress', 4, N'در حال انجام')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'10005' AND [LookupCategoryId] = 1)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'10005', 1, N'Scheduled', 5, N'برنامه ریزی شده')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'10006' AND [LookupCategoryId] = 1)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'10006', 1, N'DeviceDisconnected', 6, N'در انتظار اتصال دستگاه')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'10007' AND [LookupCategoryId] = 1)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'10007', 1, N'Incomplete', 7, N'ناقص')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'10008' AND [LookupCategoryId] = 1)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'10008', 1, N'Recurring', 8, N'تکرار شونده')

--------------------------------
--Task Types
--------------------------------

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'11001' AND [LookupCategoryId] = 2)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'11001', 2, N'Send Users', 1, N'ارسال کاربران')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'11002' AND [LookupCategoryId] = 2)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'11002', 2, N'Delete Users', 1, N'حذف کاربران')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'11101' AND [LookupCategoryId] = 2)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'11101', 2, N'SendDevices', 6, N'ارسال اطلاعات دستگاه ها')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'11501' AND [LookupCategoryId] = 2)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'11501', 2, N'SendFoods', 2, N'ارسال غذا ها')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'11502' AND [LookupCategoryId] = 2)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'11502', 2, N'SendMeals', 3, N'ارسال وعده های غذایی')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'11503' AND [LookupCategoryId] = 2)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'11503', 2, N'SendRestaurants', 4, N'ارسال رستوران ها')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'11504' AND [LookupCategoryId] = 2)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'11504', 2, N'SendReservations', 5, N'ارسال اطلاعات رزرو ها')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'11505' AND [LookupCategoryId] = 2)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'11505', 2, N'GetServeLogs', 7, N'دریافت اطلاعات سرو غذا')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'11506' AND [LookupCategoryId] = 2)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'11506', 2, N'SendMealTimings', 8, N'ارسال زمان وعده های غذایی')


IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'11102' AND [LookupCategoryId] = 2)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'11102', 2, N'UpgradeDeviceFirmware', 9, N'آپدیت فریم ورک')
IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'11103' AND [LookupCategoryId] = 2)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'11103', 2, N'UnlockDevice',10, N'بازکردن قفل دستگاه')
IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'11104' AND [LookupCategoryId] = 2)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'11104', 2, N'SendTimeZoneToTerminal',11, N'فرستادن ساعت به دستگاه')
IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'11105' AND [LookupCategoryId] = 2)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'11105', 2, N'SendAccessGroupToTerminal',12, N'فرستادن گروه دسترسی به دستگاه')
IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'11106' AND [LookupCategoryId] = 2)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'11106', 2, N'RetrieveUserFromTerminal',13, N'تخلیه فرد از دستگاه')
IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'11107' AND [LookupCategoryId] = 2)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'11107', 2, N'RetrieveAllUsersFromTerminal',14, N'تخلیه افراد از دستگاه')
IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'11108' AND [LookupCategoryId] = 2)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'11108', 2, N'OpenDoor',15, N'بازکردن گیت متصل به دستکاه')
IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'11109' AND [LookupCategoryId] = 2)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'11109', 2, N'LockDevice',16, N'قفل کردن دستگاه')
IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'11110' AND [LookupCategoryId] = 2)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'11110', 2, N'EnrollFromTerminal',17, N'تعریف کاربر از دستگاه ')
IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'11111' AND [LookupCategoryId] = 2)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'11111', 2, N'DeleteUserFromTerminal',18, N'حذف فرد از دستگاه')
IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'11112' AND [LookupCategoryId] = 2)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'11112', 2, N'EnrollFaceFromTerminal',19, N'تعریف چهره کاربر از دستگاه')


IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'11601' AND [LookupCategoryId] = 2)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'11601', 2, N'SendBlackList', 19, N'ارسال بلک لیست ')


IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'11401' AND [LookupCategoryId] = 2)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'11401', 2, N'ClearLog', 20, N'پاک کردن تردد ')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'11402' AND [LookupCategoryId] = 2)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'11402', 2, N'GetLogsInPeriod', 20, N'تخلیه تردد در بازه زمانی ')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'11403' AND [LookupCategoryId] = 2)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'11403', 2, N'GetLogs', 20, N'تخلیه تردد ')


--------------------------------
--Task Item Types
--------------------------------

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'12001' AND [LookupCategoryId] = 3)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'12001', 3, N'Send User', 1, N'ارسال کاربر به دستگاه')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'12101' AND [LookupCategoryId] = 3)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'12101', 3, N'SendDevice', 6, N'ارسال اطلاعات دستگاه ها')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'12501' AND [LookupCategoryId] = 3)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'12501', 3, N'SendFood', 2, N'ارسال غذا ها')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'12502' AND [LookupCategoryId] = 3)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'12502', 3, N'SendMeal', 3, N'ارسال وعده های غذایی')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'12503' AND [LookupCategoryId] = 3)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'12503', 3, N'SendRestaurant', 4, N'ارسال رستوران ها')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'12504' AND [LookupCategoryId] = 3)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'12504', 3, N'SendReservation', 5, N'ارسال اطلاعات رزرو ها')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'12505' AND [LookupCategoryId] = 3)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'12505', 3, N'GetServeLogs', 7, N'دریافت اطلاعات سرو غذا')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'12506' AND [LookupCategoryId] = 3)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'12506', 3, N'SendMealTiming', 8, N'ارسال زمان وعده ی غذایی')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'12507' AND [LookupCategoryId] = 3)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'12507', 3, N'GetServeLogsInPeriod', 9, N'دریافت اطلاعات سرو غذا در بازه زمانی')


IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'12102' AND [LookupCategoryId] = 3)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'12102', 3, N'UpgradeDeviceFirmware', 9, N'آپدیت فریم ورک')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'12103' AND [LookupCategoryId] = 3)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'12103', 3, N'UnlockDevice',10, N'بازکردن قفل دستگاه')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'12104' AND [LookupCategoryId] = 3)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'12104', 3, N'SendTimeZoneToTerminal',11, N'فرستادن ساعت به دستگاه')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'12105' AND [LookupCategoryId] = 3)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'12105', 3, N'SendAccessGroupToTerminal',12, N'فرستادن گروه دسترسی به دستگاه')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'12106' AND [LookupCategoryId] = 3)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'12106', 3, N'RetrieveUserFromTerminal',13, N'تخلیه فرد از دستگاه')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'12107' AND [LookupCategoryId] = 3)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'12107', 3, N'RetrieveAllUsersFromTerminal',14, N'تخلیه افراد از دستگاه')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'12108' AND [LookupCategoryId] = 3)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'12108', 3, N'OpenDoor',15, N'بازکردن گیت متصل به دستکاه')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'12109' AND [LookupCategoryId] = 3)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'12109', 3, N'LockDevice',16, N'قفل کردن دستگاه')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'12110' AND [LookupCategoryId] = 3)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'12110', 3, N'EnrollFromTerminal',17, N'تعریف کاربر از دستگاه ')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'12111' AND [LookupCategoryId] = 3)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'12111', 3, N'DeleteUserFromTerminal',18, N'حذف فرد از دستگاه')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'12112' AND [LookupCategoryId] = 3)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'12112', 3, N'EnrollFaceFromTerminal',19, N'تعریف چهره کاربر از دستگاه ')


IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'12601' AND [LookupCategoryId] = 3)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'12601', 3, N'SendBlakList',20, N'ارسال بلک لیست به دستگاه')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'12401' AND [LookupCategoryId] = 3)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'12401', 3, N'ClearLog',21, N'پاک کردن تردد')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'12402' AND [LookupCategoryId] = 3)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'12402', 3, N'GetLogsInPeriod',22, N'تخلیه تردد در بازه زمانی')
IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'12403' AND [LookupCategoryId] = 3)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'12403', 3, N'GetLogs',23, N'تخلیه تردد')

--------------------------------
--Task Priorities
--------------------------------

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'13001' AND [LookupCategoryId] = 4)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'13001', 4, N'Lowest', 1, N'کمترین اولویت')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'13002' AND [LookupCategoryId] = 4)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'13002', 4, N'Low', 2, N'اولویت پایین')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'13003' AND [LookupCategoryId] = 4)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'13003', 4, N'Medium', 3, N'اولویت متوسط')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'13004' AND [LookupCategoryId] = 4)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'13004', 4, N'High', 4, N'اولویت بالا')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'13005' AND [LookupCategoryId] = 4)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'13005', 4, N'Immediate', 5, N'فوری')

--------------------------------
--Finger Template Index
--------------------------------

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'14001' AND [LookupCategoryId] = 5)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'14001', 5, N'RightThumb', 1, N'شست راست')
ELSE
	UPDATE [dbo].[Lookup] SET [Name] = N'RightThumb', [OrderIndex] = 1, [Description] = N'شست راست' WHERE [Code] = N'14001' AND [LookupCategoryId] = 5

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'14002' AND [LookupCategoryId] = 5)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'14002', 5, N'RightIndex', 2, N'اشاره راست')
	
IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'14003' AND [LookupCategoryId] = 5)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'14003', 5, N'RightMiddle', 3, N'میانی راست')
	
IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'14004' AND [LookupCategoryId] = 5)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'14004', 5, N'RightRing', 4, N'حلقه راست')
	
IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'14005' AND [LookupCategoryId] = 5)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'14005', 5, N'RightLittle', 5, N'کوچک راست')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'14006' AND [LookupCategoryId] = 5)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'14006', 5, N'LeftLittle', 6, N'کوچک چپ')
ELSE
	UPDATE [dbo].[Lookup] SET [Name] = N'LeftLittle', [OrderIndex] = 6, [Description] = N'کوچک چپ' WHERE [Code] = N'14006' AND [LookupCategoryId] = 5
	
IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'14007' AND [LookupCategoryId] = 5)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'14007', 5, N'LeftRing', 7, N'حلقه چپ')
ELSE
	UPDATE [dbo].[Lookup] SET [Name] = N'LeftRing', [OrderIndex] = 7, [Description] = N'حلقه چپ' WHERE [Code] = N'14007' AND [LookupCategoryId] = 5

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'14008' AND [LookupCategoryId] = 5)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'14008', 5, N'LeftMiddle', 8, N'میانی چپ')
ELSE
	UPDATE [dbo].[Lookup] SET [Name] = N'LeftMiddle', [OrderIndex] = 8, [Description] = N'میانی چپ' WHERE [Code] = N'14008' AND [LookupCategoryId] = 5

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'14009' AND [LookupCategoryId] = 5)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'14009', 5, N'LeftIndex', 9, N'اشاره چپ')
ELSE
	UPDATE [dbo].[Lookup] SET [Name] = N'LeftIndex', [OrderIndex] = 9, [Description] = N'اشاره چپ' WHERE [Code] = N'14009' AND [LookupCategoryId] = 5

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'140010' AND [LookupCategoryId] = 5)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'140010', 5, N'LeftThumb', 10, N'شست چپ')
ELSE
	UPDATE [dbo].[Lookup] SET [Name] = N'LeftThumb', [OrderIndex] = 10, [Description] = N'شست چپ' WHERE [Code] = N'140010' AND [LookupCategoryId] = 5

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'140011' AND [LookupCategoryId] = 5)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'140011', 5, N'Unknown', 0, N'تعیین نشده')

--------------------------------
--Device Brands
--------------------------------

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'15001' AND [LookupCategoryId] = 6)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'15001', 6, N'Virdi', 1, N'Unicom Communication Co. ,Unis Serivces')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'15002' AND [LookupCategoryId] = 6)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'15002', 6, N'EOS', 2, N'Elm-o-Sanat Devices')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'15003' AND [LookupCategoryId] = 6)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'15003', 6, N'Suprema', 3, N'Suprema Co.')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'15004' AND [LookupCategoryId] = 6)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'15004', 6, N'ZK', 4, N'ZKTeco Co.')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'15005' AND [LookupCategoryId] = 6)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'15005', 6, N'PW', 5, N'Processing World Co.')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'15006' AND [LookupCategoryId] = 6)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'15006', 6, N'Maxa', 6, N'Kasra Co. Restaurant Managment System')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'15007' AND [LookupCategoryId] = 6)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'15007', 6, N'Shahab', 7, N'Shahab license detector')

--------------------------------
--Log Events
--------------------------------

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'16001' AND [LookupCategoryId] = 7)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'16001', 7, N'Connect', 1, N'اتصال ')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'16002' AND [LookupCategoryId] = 7)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'16002', 7, N'DisconnectConnect', 2, N' عدم اتصال ')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'16003' AND [LookupCategoryId] = 7)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'16003', 7, N'Athorized', 3, N'مجاز')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'16004' AND [LookupCategoryId] = 7)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'16004', 7, N'Unathorized', 4, N'غیرمجاز')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'16005' AND [LookupCategoryId] = 7)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'16005', 7, N'UserAddedToDevice', 5, N'اضافه شدن کاربر به دستگاه')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'16006' AND [LookupCategoryId] = 7)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'16006', 7, N'UserRemovedFromDevice', 6, N'حذف کردن کاربر از دستگاه')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'16007' AND [LookupCategoryId] = 7)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'16007', 7, N'DeviceEnabled', 7, N'فعال کردن دستگاه')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'16008' AND [LookupCategoryId] = 7)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'16008', 7, N'DeviceDisabled', 8, N'غیر فعال کردن دستگاه')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'16009' AND [LookupCategoryId] = 7)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'16009', 7, N'EnrollSuccess', 9, N'تعریف با موفقیت')

--------------------------------
--Log Sub Events
--------------------------------

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'17001' AND [LookupCategoryId] = 8)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'17001', 8, N'Normal', 1, N'بدون حالت خاص انتخاب شده')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'17002' AND [LookupCategoryId] = 8)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'17002', 8, N'FirstFunction', 2, N'کلید F1')
	
IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'17003' AND [LookupCategoryId] = 8)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'17003', 8, N'SecondFunction', 3, N'کلید F2')
	
IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'17004' AND [LookupCategoryId] = 8)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'17004', 8, N'ThirdFunction', 4, N'کلید F3')
	
IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'17005' AND [LookupCategoryId] = 8)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'17005', 8, N'FourthFunction', 5, N'کلید F4')


--------------------------------
--Finger Template Type
--------------------------------

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'18001' AND [LookupCategoryId] = 9)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'18001', 9, N'V400', 1, N'Virdi 400 Byte template standard')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'18002' AND [LookupCategoryId] = 9)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'18002', 9, N'VX10', 2, N'ZKV10.0')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'18003' AND [LookupCategoryId] = 9)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'18003', 9, N'SU384', 3, N'Suprema 384 Byte template standard')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'18004' AND [LookupCategoryId] = 9)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'18004', 9, N'EOSSuprema', 4, N'Eos Suprema Based Finger Template')


--------------------------------
--Face Template Type
--------------------------------

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'18101' AND [LookupCategoryId] = 10)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'18101', 10, N'VFACE', 1, N'FACE FOR VIRDI')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'18102' AND [LookupCategoryId] = 10)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'18102', 10, N'VX7', 2, N'ZK Face VX7.0')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'18103' AND [LookupCategoryId] = 10)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'18103', 10, N'SFACE', 2, N'FACE FOR SUPREMA')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'18104' AND [LookupCategoryId] = 10)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'18104', 10, N'EOSHanvon', 2, N'FACE FOR EOS HANVON')


--------------------------------
-- Matching Type
--------------------------------
IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'19000' AND [LookupCategoryId] = 11)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'19000', 11, N'Unknown', 1, N'نامشخص')

	IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'0' AND [LookupCategoryId] = 11)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'0', 11, N'UnIdentify', 1, N'نامشخص غیر شناسایی')


IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'19001' AND [LookupCategoryId] = 11)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'19001', 11, N'Face', 1, N'چهره')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'19002' AND [LookupCategoryId] = 11)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'19002', 11, N'Finger', 2, N'انگشت')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'19003' AND [LookupCategoryId] = 11)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'19003', 11, N'Car', 3, N'خودرو')

IF NOT EXISTS (SELECT * FROM [dbo].[Lookup] WHERE [Code] = N'19004' AND [LookupCategoryId] = 11)
	INSERT [dbo].[Lookup] ([Code], [LookupCategoryId], [Name], [OrderIndex], [Description]) VALUES (N'19004', 11, N'Card', 4, N'کارت')