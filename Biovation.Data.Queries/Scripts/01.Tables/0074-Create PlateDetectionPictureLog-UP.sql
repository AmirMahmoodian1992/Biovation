CREATE TABLE [dbo].[PlateDetectionPictureLog](
	[LogId] [bigint] NOT NULL,
	[FullImage] [varbinary](max) NULL,
	[PlateImage] [varbinary](max) NULL,
 CONSTRAINT [PK_PlateDetectionPictureLog] PRIMARY KEY CLUSTERED 
(
	[LogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[PlateDetectionPictureLog]  WITH CHECK ADD  CONSTRAINT [FK_PlateDetectionPictureLog_PlateDetectionLog] FOREIGN KEY([LogId])
REFERENCES [dbo].[PlateDetectionLog] ([Id])
GO

ALTER TABLE [dbo].[PlateDetectionPictureLog] CHECK CONSTRAINT [FK_PlateDetectionPictureLog_PlateDetectionLog]
GO



