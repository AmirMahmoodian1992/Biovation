IF COL_LENGTH('dbo.Device', 'RestaurantId') IS NULL
BEGIN
ALTER TABLE dbo.[Device]
ADD
[RestaurantId] [int] NULL

ALTER TABLE [dbo].[Device]  WITH CHECK ADD  CONSTRAINT [FK_Device_Restaurant] FOREIGN KEY([RestaurantId])
REFERENCES [Rst].[Restaurant] ([Id])

END
GO

ALTER TABLE [dbo].[Device] CHECK CONSTRAINT [FK_Device_Restaurant]
GO