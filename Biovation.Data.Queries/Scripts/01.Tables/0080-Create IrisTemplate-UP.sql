CREATE TABLE [dbo].[IrisTemplate](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[Template] [varbinary](max) NOT NULL,
	[Index] [int] NOT NULL,
	[EnrollQuality] [int] NULL,
	[Size] [int] NULL,
	[IrisTemplateType] [int] NULL,
	[CheckSum] [int] NULL,
	[SecurityLevel] [int] NULL,
	[CreateBy] [bigint] NULL,
	[CreateAt] [datetime] NULL,
	[UpdateBy] [bigint] NULL,
	[UpdateAt] [datetime] NULL,
 CONSTRAINT [PK_IrisTemplate] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO