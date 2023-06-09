
CREATE TABLE [dbo].[Log](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DeviceId] [int] NOT NULL,
	[EventId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[DateTime] [datetime] NOT NULL,
	[Ticks] [bigint] NOT NULL,
	[SubEvent] [int] NOT NULL,
	[TNAEvent] [int] NOT NULL,
	[MatchingType] [int] NOT NULL,
	[Image] [nvarchar](2000) NULL
 CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[DeviceId] ASC,
	[EventId] ASC,
	[Ticks] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Log]  WITH CHECK ADD  CONSTRAINT [FK_Log_Device] FOREIGN KEY([DeviceId])
REFERENCES [dbo].[Device] ([Id])
GO
ALTER TABLE [dbo].[Log] CHECK CONSTRAINT [FK_Log_Device]
GO
