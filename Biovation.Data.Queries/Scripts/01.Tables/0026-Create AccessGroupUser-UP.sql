
CREATE TABLE [dbo].[AccessGroupUser](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AccessGroupId] [int] NOT NULL,
	[UserGroupId] [int] NOT NULL,
 CONSTRAINT [PK_AccessGroupUser] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[AccessGroupUser]  WITH CHECK ADD  CONSTRAINT [FK_AccessGroupUser_AccessGroup] FOREIGN KEY([AccessGroupId])
REFERENCES [dbo].[AccessGroup] ([Id])
GO
ALTER TABLE [dbo].[AccessGroupUser] CHECK CONSTRAINT [FK_AccessGroupUser_AccessGroup]
GO
ALTER TABLE [dbo].[AccessGroupUser]  WITH CHECK ADD  CONSTRAINT [FK_AccessGroupUser_UserGroup] FOREIGN KEY([UserGroupId])
REFERENCES [dbo].[UserGroup] ([Id])
GO
ALTER TABLE [dbo].[AccessGroupUser] CHECK CONSTRAINT [FK_AccessGroupUser_UserGroup]
GO
