
CREATE TABLE [dbo].[Task](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TaskTypeCode] [nvarchar](50) NOT NULL,
	[PriorityLevelCode] [nvarchar](50) NOT NULL,
	[CreatedBy] [int] NULL,
	[CreatedAt] [datetimeoffset](7) NOT NULL,
	[DeviceBrandId] [int] NULL,
 CONSTRAINT [PK_Task] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Task] ADD  CONSTRAINT [DF_Table_1_Priority]  DEFAULT ((3)) FOR [PriorityLevelCode]
GO

ALTER TABLE [dbo].[Task] ADD  CONSTRAINT [DF_Task_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO

--ALTER TABLE [dbo].[Task]  WITH CHECK ADD  CONSTRAINT [FK_Task_DeviceBrand] FOREIGN KEY([DeviceBrandId])
--REFERENCES [dbo].[DeviceBrand] ([Id])
--GO

--ALTER TABLE [dbo].[Task] CHECK CONSTRAINT [FK_Task_DeviceBrand]
--GO

ALTER TABLE [dbo].[Task]  WITH CHECK ADD  CONSTRAINT [FK_Task_User] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[User] ([Id])
GO

ALTER TABLE [dbo].[Task] CHECK CONSTRAINT [FK_Task_User]
GO

ALTER TABLE [dbo].[Task]  WITH CHECK ADD  CONSTRAINT [FK_TaskPriority_Lookup] FOREIGN KEY([TaskTypeCode])
REFERENCES [dbo].[Lookup] ([Code])
GO

ALTER TABLE [dbo].[Task] CHECK CONSTRAINT [FK_TaskPriority_Lookup]
GO

ALTER TABLE [dbo].[Task]  WITH CHECK ADD  CONSTRAINT [FK_TaskType_Lookup] FOREIGN KEY([TaskTypeCode])
REFERENCES [dbo].[Lookup] ([Code])
GO

ALTER TABLE [dbo].[Task] CHECK CONSTRAINT [FK_TaskType_Lookup]
GO