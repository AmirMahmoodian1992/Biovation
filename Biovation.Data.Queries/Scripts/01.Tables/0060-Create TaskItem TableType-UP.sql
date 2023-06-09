IF type_id('[dbo].[TaskItemTable]') IS NULL
CREATE TYPE [dbo].[TaskItemTable] AS TABLE(
	[Id] [int] NOT NULL,
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
	[IsParallelRestricted] [bit] NOT NULL
)

