
CREATE TABLE [dbo].[OfflineEvents](
	[Id] [int] NOT NULL IDENTITY(1, 1),
	[DeviceId] [int] NOT NULL,
	[Data] [nvarchar](500) NOT NULL,
 CONSTRAINT [PK_OfflineEvents] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
--ALTER TABLE [dbo].[OfflineEvents]  WITH CHECK ADD  CONSTRAINT [FK_OfflineEvents_Device] FOREIGN KEY([DeviceId])
--REFERENCES [dbo].[Device] ([Id])
--GO
--ALTER TABLE [dbo].[OfflineEvents] CHECK CONSTRAINT [FK_OfflineEvents_Device]
--GO
