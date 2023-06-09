
CREATE PROCEDURE [dbo].[UpdateDeviceDynamicPropertyValue]
@Id INT, @DeviceId INT, @DynamicPropertyId INT, @Value NVARCHAR (100)
AS
BEGIN
    IF EXISTS (SELECT DeviceId
               FROM   [DeviceDynamicPropertyValue]
               WHERE  Id = @Id
                      AND DeviceId = @DeviceId
                      AND DynamicPropertyId = @DynamicPropertyId)
        BEGIN
            UPDATE [dbo].[DeviceDynamicPropertyValue]
            SET    Value = @Value
            WHERE  Id = @Id
                   AND DeviceId = @DeviceId
                   AND DynamicPropertyId = @DynamicPropertyId;
        END
END
GO
