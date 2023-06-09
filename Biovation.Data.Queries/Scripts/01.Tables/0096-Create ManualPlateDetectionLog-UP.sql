
CREATE TABLE [Rly].[ManualPlateDetectionLog]
(
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DetectorId] [int] NOT NULL,
	[UserId] [bigint] NOT NULL,
	[ParentLogId] [bigint] NOT NULL,
	[EventId] [nvarchar](50) NOT NULL,
	[LicensePlateId] [int] NOT NULL,
	[LogDateTime] [datetime] NOT NULL,
	[Ticks] [bigint] NOT NULL,
	[DetectionPrecision] [tinyint] NOT NULL,
	[SuccessTransfer] [bit] NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
	[InOrOut] [TINYINT] NULL
 CONSTRAINT [PK_PlateDetectionLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


