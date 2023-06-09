
CREATE TABLE [dbo].[FingerTemplate](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[TemplateIndex] [int] NOT NULL,
	[FingerIndex] [int] NOT NULL,
	[Template] [varbinary](max) NOT NULL,
	[Index] [int] NOT NULL,
	[Duress] [bit] NOT NULL,
	[CheckSum] [int] NOT NULL,
	[SecurityLevel] [int] NOT NULL,
	[EnrollQuality] [int] NOT NULL,
	[Size] [int] NOT NULL,
	[CreateBy] [bigint] NULL,
	[CreateAt] [datetime] NULL,
	[UpdateBy] [bigint] NULL,
	[UpdateAt] [datetime] NULL,
 CONSTRAINT [PK_Finger_Template] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[FingerTemplate]  WITH CHECK ADD  CONSTRAINT [FK_FingerTemplate_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[FingerTemplate] CHECK CONSTRAINT [FK_FingerTemplate_User]
GO
