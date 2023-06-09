
CREATE TABLE [dbo].[TaskItem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TaskId] [int] NOT NULL,
	[TaskItemTypeCode] [nvarchar](50) NOT NULL,
	[PriorityLevelCode] [nvarchar](50) NOT NULL,
	[StatusCode] [nvarchar](50) NOT NULL,
	[DeviceId] [int] NULL,
	[Data] [nvarchar](max) NOT NULL,
	[Result] [nvarchar](max) NULL,
	[OrderIndex] [int] NOT NULL,
	[IsScheduled] [bit] NOT NULL,
	[DueDate] [datetimeoffset](7) NOT NULL,
	[IsParallelRestricted] [bit] NOT NULL,
 CONSTRAINT [PK_TaskItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

IF OBJECT_ID('DF_TaskItem_IsScheduled', 'D') IS NULL
BEGIN
ALTER TABLE [dbo].[TaskItem] ADD  CONSTRAINT [DF_TaskItem_IsScheduled]  DEFAULT ((0)) FOR [IsScheduled]
END
GO

IF OBJECT_ID('DF_TaskItem_DueDate', 'D') IS NULL
BEGIN
ALTER TABLE [dbo].[TaskItem] ADD  CONSTRAINT [DF_TaskItem_DueDate]  DEFAULT (getdate()) FOR [DueDate]
END
GO

IF OBJECT_ID('DF_TaskItem_IsParallelRestricted', 'D') IS NULL
BEGIN
ALTER TABLE [dbo].[TaskItem] ADD  CONSTRAINT [DF_TaskItem_IsParallelRestricted]  DEFAULT ((1)) FOR [IsParallelRestricted]
END
GO


--IF (OBJECT_ID('dbo.FK_TaskItem_Device', 'F') IS NULL)
--BEGIN
--ALTER TABLE [dbo].[TaskItem]  WITH CHECK ADD  CONSTRAINT [FK_TaskItem_Device] FOREIGN KEY([DeviceId])
--REFERENCES [dbo].[Device] ([Id])
--END
--GO

--ALTER TABLE [dbo].[TaskItem] CHECK CONSTRAINT [FK_TaskItem_Device]
--GO


IF (OBJECT_ID('dbo.FK_TaskItemPriority_Lookup', 'F') IS NULL)
BEGIN
ALTER TABLE [dbo].[TaskItem]  WITH CHECK ADD  CONSTRAINT [FK_TaskItemPriority_Lookup] FOREIGN KEY([PriorityLevelCode])
REFERENCES [dbo].[Lookup] ([Code])
END
GO

ALTER TABLE [dbo].[TaskItem] CHECK CONSTRAINT [FK_TaskItemPriority_Lookup]
GO

IF (OBJECT_ID('dbo.FK_TaskItemStatus_Lookup', 'F') IS NULL)
BEGIN
ALTER TABLE [dbo].[TaskItem]  WITH CHECK ADD  CONSTRAINT [FK_TaskItemStatus_Lookup] FOREIGN KEY([StatusCode])
REFERENCES [dbo].[Lookup] ([Code])
END
GO

ALTER TABLE [dbo].[TaskItem] CHECK CONSTRAINT [FK_TaskItemStatus_Lookup]
GO

IF (OBJECT_ID('dbo.FK_TaskItemType_Lookup', 'F') IS NULL)
BEGIN
ALTER TABLE [dbo].[TaskItem]  WITH CHECK ADD  CONSTRAINT [FK_TaskItemType_Lookup] FOREIGN KEY([TaskItemTypeCode])
REFERENCES [dbo].[Lookup] ([Code])
END
GO

ALTER TABLE [dbo].[TaskItem] CHECK CONSTRAINT [FK_TaskItemType_Lookup]
GO

IF (OBJECT_ID('dbo.FK_TaskItem_Task', 'F') IS NULL)
BEGIN
ALTER TABLE [dbo].[TaskItem]  WITH CHECK ADD  CONSTRAINT [FK_TaskItem_Task] FOREIGN KEY([TaskId])
REFERENCES [dbo].[Task] ([Id])
END
GO

ALTER TABLE [dbo].[TaskItem] CHECK CONSTRAINT [FK_TaskItem_Task]
GO