
CREATE TABLE [dbo].[FingerTemplateType](
	[Id] [int] NOT NULL,
	[BrandId] [int] NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](150) NULL,
 CONSTRAINT [PK_FingerTemplateType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[FingerTemplateType]  WITH CHECK ADD  CONSTRAINT [FK_FingerTemplateType_DeviceBrand] FOREIGN KEY([BrandId])
REFERENCES [dbo].[DeviceBrand] ([ID])
GO

ALTER TABLE [dbo].[FingerTemplateType] CHECK CONSTRAINT [FK_FingerTemplateType_DeviceBrand]
GO