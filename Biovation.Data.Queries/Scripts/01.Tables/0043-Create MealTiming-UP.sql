
CREATE TABLE [Rst].[MealTiming](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MealId] [int] NOT NULL,
	[DeviceId] [int] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[StartTimeInMinutes] [int] NOT NULL,
	[EndTimeInMinutes] [int] NOT NULL,
 CONSTRAINT [PK_MealTiming] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [Rst].[MealTiming]  WITH CHECK ADD  CONSTRAINT [FK_MealTiming_Meal] FOREIGN KEY([MealId])
REFERENCES [Rst].[Meal] ([Id])
GO

ALTER TABLE [Rst].[MealTiming] CHECK CONSTRAINT [FK_MealTiming_Meal]
GO

ALTER TABLE [Rst].[MealTiming]  WITH CHECK ADD  CONSTRAINT [FK_MealTiming_Device] FOREIGN KEY([DeviceId])
REFERENCES [dbo].[Device] ([Id])
GO

ALTER TABLE [Rst].[MealTiming] CHECK CONSTRAINT [FK_MealTiming_Device]
GO