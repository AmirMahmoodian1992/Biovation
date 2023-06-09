CREATE TABLE [dbo].[BlackList](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[DeviceId] [int] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[IsDeleted] [bit] NULL,
	[Description] [nvarchar](200) NULL,
 CONSTRAINT [PK_BlackList] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[BlackList]  WITH CHECK ADD  CONSTRAINT [FK_BlackList_Device] FOREIGN KEY([DeviceId])
REFERENCES [dbo].[Device] ([Id])
GO

ALTER TABLE [dbo].[BlackList] CHECK CONSTRAINT [FK_BlackList_Device]
GO

ALTER TABLE [dbo].[BlackList]  WITH CHECK ADD  CONSTRAINT [FK_BlackList_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO

ALTER TABLE [dbo].[BlackList] CHECK CONSTRAINT [FK_BlackList_User]
GO