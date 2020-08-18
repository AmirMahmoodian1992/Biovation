CREATE TABLE [dbo].[LicensePlate](
	[EntityId] [int] NOT NULL,
	[LicensePlate] [nvarchar](50) NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[LicensePlate]  WITH CHECK ADD  CONSTRAINT [FK_LicensePlate_Entity] FOREIGN KEY([EntityId])
REFERENCES [dbo].[Entity] ([id])
GO

ALTER TABLE [dbo].[LicensePlate] CHECK CONSTRAINT [FK_LicensePlate_Entity]
GO









