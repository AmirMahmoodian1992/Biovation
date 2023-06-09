
CREATE TABLE [dbo].[AccessGroupDevice](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AccessGroupId] [int] NOT NULL,
	[DeviceGroupId] [int] NOT NULL,
 CONSTRAINT [PK_AccessGroupDevice] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[AccessGroupDevice]  WITH CHECK ADD  CONSTRAINT [FK_AccessGroupDevice_AccessGroup] FOREIGN KEY([AccessGroupId])
REFERENCES [dbo].[AccessGroup] ([Id])
GO
ALTER TABLE [dbo].[AccessGroupDevice] CHECK CONSTRAINT [FK_AccessGroupDevice_AccessGroup]
GO
ALTER TABLE [dbo].[AccessGroupDevice]  WITH CHECK ADD  CONSTRAINT [FK_AccessGroupDevice_DeviceGroup] FOREIGN KEY([DeviceGroupId])
REFERENCES [dbo].[DeviceGroup] ([Id])
GO
ALTER TABLE [dbo].[AccessGroupDevice] CHECK CONSTRAINT [FK_AccessGroupDevice_DeviceGroup]
GO
