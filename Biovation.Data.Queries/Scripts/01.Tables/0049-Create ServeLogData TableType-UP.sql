IF type_id('[dbo].[ServeLogTable]') IS NULL
CREATE TYPE [dbo].[ServeLogTable] AS TABLE(
	[Id] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[FoodId] [int] NOT NULL,
	[MealId] [int] NOT NULL,
	[DeviceId] [int] NOT NULL,
	[StatusId] [int] NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
	[IsSynced] [bit] NOT NULL
)
