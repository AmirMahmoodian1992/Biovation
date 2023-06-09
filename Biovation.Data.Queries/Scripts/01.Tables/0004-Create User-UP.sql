
CREATE TABLE [dbo].[User](
	[Id] [int] NOT NULL,
	[FirstName] [nvarchar](200) NULL,
	[SurName] [nvarchar](200) NULL,
	[UserName] [nvarchar](200) NULL,
	[StartDate] [smalldatetime] NOT NULL,
	[EndDate] [smalldatetime] NOT NULL,
	[RegisterDate] [smalldatetime] NOT NULL CONSTRAINT [DF_User_RegisterDate]  DEFAULT (getdate()),
	[IsActive] [bit] NOT NULL,
	[IsAdmin] [bit] NOT NULL,
	[AdminLevel] [int] NOT NULL,
	[Password] [nvarchar](max) NULL,
	[PasswordBytes] [binary](50) NULL,
	[AuthMode] [int] NULL,
	[IsMasterAdmin] [bit] NULL,
	[Image] [nvarchar](2000) NULL
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
