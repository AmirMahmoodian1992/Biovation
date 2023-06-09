
CREATE TABLE [dbo].[Exception](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Language] [tinyint] NULL,
	[Code] [int] NULL,
	[Title] [nvarchar](50) NULL,
	[Message] [nvarchar](350) NULL,
 CONSTRAINT [PK_Exception] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
