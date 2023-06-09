
CREATE TABLE [dbo].[DeviceDynamicPropertyName](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[TypeId] [int] NOT NULL,
	[ModelId] [int] NOT NULL,
	[Description] [varchar](250) NOT NULL,
 CONSTRAINT [PK_DeviceDynamicProperty] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[DeviceDynamicPropertyName]  WITH CHECK ADD  CONSTRAINT [FK_DeviceDynamicPropertyName_DeviceModel] FOREIGN KEY([ModelId])
REFERENCES [dbo].[DeviceModel] ([Id])
GO
ALTER TABLE [dbo].[DeviceDynamicPropertyName] CHECK CONSTRAINT [FK_DeviceDynamicPropertyName_DeviceModel]
GO
ALTER TABLE [dbo].[DeviceDynamicPropertyName]  WITH CHECK ADD  CONSTRAINT [FK_DeviceDynamicPropertyName_DynamicPropertyTypes] FOREIGN KEY([TypeId])
REFERENCES [dbo].[DynamicPropertyTypes] ([Id])
GO
ALTER TABLE [dbo].[DeviceDynamicPropertyName] CHECK CONSTRAINT [FK_DeviceDynamicPropertyName_DynamicPropertyTypes]
GO
