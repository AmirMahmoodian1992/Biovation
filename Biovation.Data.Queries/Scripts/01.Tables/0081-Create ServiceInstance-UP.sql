
CREATE TABLE [dbo].[ServiceInstance](
	[Id] [nvarchar](max) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Version] [nvarchar](max) NULL,
	[LastUpTime] [datetime] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Health] [bit] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

