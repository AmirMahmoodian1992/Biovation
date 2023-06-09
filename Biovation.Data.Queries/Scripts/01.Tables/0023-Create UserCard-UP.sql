
CREATE TABLE [dbo].[UserCard](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CardNum] [nvarchar](50) NOT NULL,
	[UserId] [int] NULL,
	[DataCheck] [int] NULL,
	[IsActive] [bit] NOT NULL CONSTRAINT [DF_UserCard_IsActive]  DEFAULT ((1)),
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_UserCard_IsDelete]  DEFAULT ((0)),
	[CreatedAt] [datetime] NULL,
	[ModifiedAt] [datetime] NULL
) ON [PRIMARY]

GO
