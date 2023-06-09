
CREATE TABLE [dbo].[AuthModeMap](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BioCode] [int] NOT NULL,
	[BrandId] [int] NULL,
	[AuthMode] [int] NULL,
	[BioTitle] [nvarchar](500) NULL,
 CONSTRAINT [PK_AuthModeMap] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'1 : FP , 2 : Card , 3 : Face , 4 : Password ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'AuthModeMap', @level2type=N'COLUMN',@level2name=N'BioCode'
GO
