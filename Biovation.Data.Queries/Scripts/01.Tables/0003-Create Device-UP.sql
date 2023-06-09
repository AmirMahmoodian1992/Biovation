
CREATE TABLE [dbo].[Device](
	[Id] [int] NOT NULL IDENTITY(1, 1),
	[Code] [bigint] NOT NULL,
	[DeviceModelId] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Active] [bit] NOT NULL,
	[IPAddress] [nvarchar](16) NOT NULL,
	[Port] [int] NOT NULL,
	[MacAddress] [nvarchar](100) NULL,
	[RegisterDate] [smalldatetime] NOT NULL,
	[HardwareVersion] [nvarchar](100) NULL,
	[FirmwareVersion] [nvarchar](100) NULL,
	[DeviceLockPassword] [nvarchar](16) NULL,
	[SSL] [bit] NOT NULL CONSTRAINT [DF_Device_SSL]  DEFAULT ((0)),
	[TimeSync] [bit] NOT NULL CONSTRAINT [DF_Device_TimeSync]  DEFAULT ((0)),
	[SerialNumber] [nvarchar](100) NULL,
	[DeviceTypeId] [int] NULL,
 CONSTRAINT [PK_Device] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Device]  WITH CHECK ADD  CONSTRAINT [FK_Device_DeviceModel] FOREIGN KEY([DeviceModelId])
REFERENCES [dbo].[DeviceModel] ([Id])
GO
ALTER TABLE [dbo].[Device] CHECK CONSTRAINT [FK_Device_DeviceModel]
GO
