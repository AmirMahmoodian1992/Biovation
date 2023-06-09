IF type_id('[dbo].[LogTable]') IS NULL
CREATE TYPE [dbo].[LogTable] AS TABLE(
	[Id] [int] NOT NULL,
	[DeviceId] [int] NOT NULL,
	[DeviceCode] [bigint] NOT NULL,
	[EventId] [int] NOT NULL,
	[UserId] [bigint] NOT NULL,
	[DateTime] [datetime] NOT NULL,
	[Ticks] [bigint] NOT NULL,
	[SubEvent] [int] NOT NULL,
	[TNAEvent] [int] NOT NULL,
	[MatchingType] [int] NOT NULL,
	[SuccessTransfer] [bit] NULL
)
GO

