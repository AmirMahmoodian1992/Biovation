
CREATE TABLE [dbo].[DeviceModel](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[BrandId] [int] NOT NULL,
	[ManufactureCode] [int] NOT NULL CONSTRAINT [DF_DeviceModel_ManufactureCode]  DEFAULT ((0)),
	[GetLogMethodType] [int] NOT NULL CONSTRAINT [DF_DeviceModel_GetLogMethodType]  DEFAULT ((0)),
	[Description] [nvarchar](250) NULL,
 CONSTRAINT [PK_DeviceModel] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[DeviceModel]  WITH CHECK ADD  CONSTRAINT [FK_DeviceModel_DeviceBrand] FOREIGN KEY([BrandId])
REFERENCES [dbo].[DeviceBrand] ([Id])
GO
ALTER TABLE [dbo].[DeviceModel] CHECK CONSTRAINT [FK_DeviceModel_DeviceBrand]
GO
