
CREATE TABLE [dbo].[AccessGroup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](500) NOT NULL,
	[TimeZoneId] [int] NOT NULL,
	[AdminUserId] [int] NULL,
	[Description] [nvarchar](1000) NULL,
	[IsDefault] [bit] NOT NULL CONSTRAINT [DF_AccessGroup_IsDefault]  DEFAULT ((0)),
 CONSTRAINT [PK_AccessGroup] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[AccessGroup]  WITH CHECK ADD  CONSTRAINT [FK_AccessGroup_AccessGroup] FOREIGN KEY([TimeZoneId])
REFERENCES [dbo].[TimeZone] ([Id])
GO
ALTER TABLE [dbo].[AccessGroup] CHECK CONSTRAINT [FK_AccessGroup_AccessGroup]
GO
alter table [dbo].[AccessGroup] with check add constraint FK_AccessGroup_User foreign key([AdminUserId])
REFERENCES [dbo].[User] ([Id])
GO
