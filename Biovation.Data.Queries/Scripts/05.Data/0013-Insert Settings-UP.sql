
IF NOT EXISTS (SELECT * FROM [dbo].[Setting] WHERE [Key] = 'ShowLiveImageInMonitoring')
	INSERT [dbo].[Setting] ([Key], [Value], [Description]) VALUES ('ShowLiveImageInMonitoring', 'True', N'نمایش عکس گرفته شده هنگام ثبت لاگ در مانیتورینگ آنلاین')
