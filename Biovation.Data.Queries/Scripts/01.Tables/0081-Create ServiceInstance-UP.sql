
CREATE TABLE [dbo].[ServiceInstance](
	[Id] [nvarchar](max) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Version] [nvarchar](max) NULL,
	[Ip] [nvarchar](max) NULL,
	[Port] [int] NULL,
	[Description] [nvarchar](max) NULL,
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

