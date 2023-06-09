
CREATE TABLE [dbo].[TimeZoneDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TimeZoneId] [int] NOT NULL,
	[DayNumber] [int] NOT NULL,
	[FromTime] [time](7) NOT NULL,
	[ToTime] [time](7) NOT NULL,
 CONSTRAINT [PK_TimeZoneDetail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[TimeZoneDetail]  WITH NOCHECK ADD  CONSTRAINT [FK_TimeZoneDetail_TimeZone] FOREIGN KEY([TimeZoneId])
REFERENCES [dbo].[TimeZone] ([Id])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[TimeZoneDetail] CHECK CONSTRAINT [FK_TimeZoneDetail_TimeZone]
GO
