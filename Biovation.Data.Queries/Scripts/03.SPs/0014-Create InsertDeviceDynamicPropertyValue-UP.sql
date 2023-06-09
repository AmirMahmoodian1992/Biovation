
CREATE PROCEDURE [dbo].[InsertDeviceDynamicPropertyValue]
@DeviceID INT, @DynamicPropertyID INT, @Value NVARCHAR (100)
AS
BEGIN
    INSERT  INTO [dbo].[DeviceDynamicPropertyValue] ([DeviceID], [DynamicPropertyID], [Value])
    VALUES                                         (@DeviceID, @DynamicPropertyID, @Value);
END
GO
