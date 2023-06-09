
CREATE PROCEDURE [dbo].[SelectDevices]
AS
BEGIN
    SELECT [Id],
		   [Code],
           [DeviceModelId],
           [Name],
           [Active],
           [IPAddress],
           [Port],
           [MacAddress],
           [RegisterDate],
           [HardwareVersion],
           [FirmwareVersion],
           [DeviceLockPassword],
           [SSL],
           [TimeSync],
           [SerialNumber]
    FROM   [dbo].[Device];
END
GO
