CREATE TABLE [Rst].[Reservation](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[FoodId] [int] NOT NULL,
	[MealId] [int] NOT NULL,
	[RestaurantId] [int] NOT NULL,
	[Count] [int] NOT NULL,
	[ReserveTime] [datetime] NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
 CONSTRAINT [PK_Reservation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [Rst].[Reservation] ADD  CONSTRAINT [DF_Reservation_TimeStamp]  DEFAULT (getdate()) FOR [TimeStamp]
GO

ALTER TABLE [Rst].[Reservation]  WITH CHECK ADD  CONSTRAINT [FK_Reservation_Food] FOREIGN KEY([FoodId])
REFERENCES [Rst].[Food] ([Id])
GO

ALTER TABLE [Rst].[Reservation] CHECK CONSTRAINT [FK_Reservation_Food]
GO

ALTER TABLE [Rst].[Reservation]  WITH CHECK ADD  CONSTRAINT [FK_Reservation_Meal] FOREIGN KEY([MealId])
REFERENCES [Rst].[Meal] ([Id])
GO

ALTER TABLE [Rst].[Reservation] CHECK CONSTRAINT [FK_Reservation_Meal]
GO

ALTER TABLE [Rst].[Reservation]  WITH CHECK ADD  CONSTRAINT [FK_Reservation_Restaurant] FOREIGN KEY([RestaurantId])
REFERENCES [Rst].[Restaurant] ([Id])
GO

ALTER TABLE [Rst].[Reservation] CHECK CONSTRAINT [FK_Reservation_Restaurant]
GO