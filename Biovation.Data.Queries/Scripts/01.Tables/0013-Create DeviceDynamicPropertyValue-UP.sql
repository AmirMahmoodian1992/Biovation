
CREATE TABLE [dbo].[DeviceDynamicPropertyValue](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DeviceId] [int] NOT NULL,
	[DynamicPropertyId] [int] NOT NULL,
	[Value] [varchar](100) NOT NULL,
 CONSTRAINT [PK_Device_DynamicProperty] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
--ALTER TABLE [dbo].[DeviceDynamicPropertyValue]  WITH CHECK ADD  CONSTRAINT [FK_Device_DynamicProperty_Device] FOREIGN KEY([DeviceId])
--REFERENCES [dbo].[Device] ([Id])
--GO
--ALTER TABLE [dbo].[DeviceDynamicPropertyValue] CHECK CONSTRAINT [FK_Device_DynamicProperty_Device]
--GO
ALTER TABLE [dbo].[DeviceDynamicPropertyValue]  WITH CHECK ADD  CONSTRAINT [FK_Device_DynamicProperty_DeviceDynamicProperty] FOREIGN KEY([DynamicPropertyId])
REFERENCES [dbo].[DeviceDynamicPropertyName] ([Id])
GO
ALTER TABLE [dbo].[DeviceDynamicPropertyValue] CHECK CONSTRAINT [FK_Device_DynamicProperty_DeviceDynamicProperty]
GO
