
CREATE TABLE [dbo].[DeviceGroupMember](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DeviceId] [int] NOT NULL,
	[GroupId] [int] NOT NULL
) ON [PRIMARY]

GO
