IF type_id('[dbo].[ReservationTable]') IS NULL
CREATE TYPE [dbo].[ReservationTable] AS TABLE(
	[Id] [int] NOT NULL,
	[UserId] [bigint] NOT NULL,
	[FoodId] [int] NOT NULL,
	[MealId] [int] NOT NULL,
	[RestaurantId] [int] NOT NULL,
	[Count] [int] NOT NULL,
	[StatusId] [int] NOT NULL,
	[ReserveTime] [datetime] NOT NULL,
	[TimeStamp] [datetime] NOT NULL
)

