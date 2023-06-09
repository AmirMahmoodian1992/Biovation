CREATE TABLE [dbo].[PlateDetectionLog](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DetectorId] [int] NOT NULL,
	[EventId] [nvarchar](50) NOT NULL,
	[LicensePlateId] [int] NOT NULL,
	[LogDateTime] [datetime] NOT NULL,
	[Ticks] [bigint] NOT NULL,
	[DetectionPrecision] [tinyint] NOT NULL,
	[SuccessTransfer] [bit] NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
 CONSTRAINT [PK_PlateDetectionLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PlateDetectionLog]  WITH CHECK ADD  CONSTRAINT [FK_PlateDetectionLog_LicensePlate] FOREIGN KEY([LicensePlateId])
REFERENCES [dbo].[Entity] ([id])
GO

ALTER TABLE [dbo].[PlateDetectionLog] CHECK CONSTRAINT [FK_PlateDetectionLog_LicensePlate]
GO

ALTER TABLE [dbo].[PlateDetectionLog]  WITH CHECK ADD  CONSTRAINT [FK_PlateDetectionLog_Device] FOREIGN KEY([DetectorId])
REFERENCES [dbo].[Device] ([id])
GO



ALTER TABLE [dbo].[PlateDetectionLog]  WITH CHECK ADD  CONSTRAINT [FK_PlateDetectionLog_Lookup] FOREIGN KEY([EventId])
REFERENCES [dbo].[LookUp] ([Code])
GO



