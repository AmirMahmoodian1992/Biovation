
CREATE TABLE [dbo].[AdminDevice](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[DeviceId] [int] NULL,
	[DeviceGroupId] [int] NULL
) ON [PRIMARY]

GO
