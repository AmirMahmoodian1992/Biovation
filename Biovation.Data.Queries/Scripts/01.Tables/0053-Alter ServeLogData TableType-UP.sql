IF OBJECT_ID('InsertServeLogsBatch', 'P') IS NOT NULL
DROP PROC [dbo].[InsertServeLogsBatch]
GO

IF type_id('[dbo].[ServeLogTable]') IS NOT NULL
	DROP TYPE [dbo].[ServeLogTable]
GO

CREATE TYPE [dbo].[ServeLogTable] AS TABLE(
	[Id] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[FoodId] [int] NOT NULL,
	[MealId] [int] NOT NULL,
	[DeviceId] [int] NOT NULL,
	[StatusId] [int] NOT NULL,
	[Count] [int] NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
	[IsSynced] [bit] NOT NULL
)