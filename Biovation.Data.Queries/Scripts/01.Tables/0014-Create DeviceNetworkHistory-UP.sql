
CREATE TABLE [dbo].[DeviceNetworkHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DeviceId] [int] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[IPAddress] [nvarchar](16) NOT NULL,
 CONSTRAINT [PK_Device_NetworkHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
--ALTER TABLE [dbo].[DeviceNetworkHistory]  WITH CHECK ADD  CONSTRAINT [FK_Device_NetworkHistory_Device] FOREIGN KEY([DeviceId])
--REFERENCES [dbo].[Device] ([Id])
--GO
--ALTER TABLE [dbo].[DeviceNetworkHistory] CHECK CONSTRAINT [FK_Device_NetworkHistory_Device]
--GO
