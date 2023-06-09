
CREATE TABLE [dbo].[UserGroupMember](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[GroupId] [int] NOT NULL,
	[UserType] [nvarchar](10) NOT NULL,
	[UserTypeTitle] [nvarchar](50) NULL,
 CONSTRAINT [PK_PersonCategoryInductee] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[UserGroupMember]  WITH CHECK ADD  CONSTRAINT [FK_UserGroupMember_UserGroup] FOREIGN KEY([GroupId])
REFERENCES [dbo].[UserGroup] ([Id])
GO
ALTER TABLE [dbo].[UserGroupMember] CHECK CONSTRAINT [FK_UserGroupMember_UserGroup]
GO
