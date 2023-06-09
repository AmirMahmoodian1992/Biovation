
CREATE TABLE [Rst].[ServeLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[FoodId] [int] NOT NULL,
	[MealId] [int] NOT NULL,
	[DeviceId] [int] NOT NULL,
	[StatusId] [int] NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
	[IsSynced] [bit] NOT NULL,
 CONSTRAINT [PK_ServeLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [Rst].[ServeLog]  WITH CHECK ADD  CONSTRAINT [FK_ServeLog_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO

ALTER TABLE [Rst].[ServeLog] CHECK CONSTRAINT [FK_ServeLog_User]
GO

ALTER TABLE [Rst].[ServeLog]  WITH CHECK ADD  CONSTRAINT [FK_ServeLog_Food] FOREIGN KEY([FoodId])
REFERENCES [Rst].[Food] ([Id])
GO

ALTER TABLE [Rst].[ServeLog] CHECK CONSTRAINT [FK_ServeLog_Food]
GO

ALTER TABLE [Rst].[ServeLog]  WITH CHECK ADD  CONSTRAINT [FK_ServeLog_Meal] FOREIGN KEY([MealId])
REFERENCES [Rst].[Meal] ([Id])
GO

ALTER TABLE [Rst].[ServeLog] CHECK CONSTRAINT [FK_ServeLog_Meal]
GO

ALTER TABLE [Rst].[ServeLog]  WITH CHECK ADD  CONSTRAINT [FK_ServeLog_Device] FOREIGN KEY([DeviceId])
REFERENCES [dbo].[Device] ([Id])
GO

ALTER TABLE [Rst].[ServeLog] CHECK CONSTRAINT [FK_ServeLog_Device]
GO

ALTER TABLE [Rst].[ServeLog] WITH CHECK ADD  CONSTRAINT [FK_ServeLog_ServeLogStatus] FOREIGN KEY([StatusId])
REFERENCES [Rst].[ServeLogStatus] ([Id])
GO

ALTER TABLE [Rst].[ServeLog] CHECK CONSTRAINT [FK_ServeLog_ServeLogStatus]
GO