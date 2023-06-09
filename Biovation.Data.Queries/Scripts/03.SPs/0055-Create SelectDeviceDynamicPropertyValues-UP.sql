
CREATE PROCEDURE [dbo].[SelectDeviceDynamicPropertyValues]
@DeviceId INT, @DynamicPropertyId INT
AS
BEGIN
    SELECT [Id],
           [DeviceId],
           [DynamicPropertyId],
           [Value]
    FROM   [dbo].[DeviceDynamicPropertyValue]
    WHERE  DeviceId = @DeviceId
           AND DynamicPropertyId = @DynamicPropertyId;
END
GO
