
CREATE PROCEDURE [dbo].[ModifyAdminDevice]
@StrXml NVARCHAR (MAX)
AS
SET NOCOUNT ON;
DECLARE @Message AS NVARCHAR (200) = N'ذخیره با موفقیت انجام شد', @Validate AS INT = 1, @xmlId AS INT, @UserId AS INT = 0,@Code AS NVARCHAR (15) = N'201';
CREATE TABLE #AdminDevice (
    [UserId] INT
);
EXECUTE sp_xml_preparedocument @xmlId OUTPUT, @StrXml;
INSERT INTO #AdminDevice ([UserId])
SELECT [UserId]
FROM   OPENXML (@xmlId, 'Root', 2) WITH ([UserId] INT) AS X;
SELECT @UserId = UserId
FROM   #AdminDevice;
CREATE TABLE #Devices (
    GroupDeviceId INT,
    TypeId        INT
);
INSERT INTO #Devices (GroupDeviceId, TypeId)
SELECT GroupDeviceId,
       TypeId
FROM   OPENXML (@xmlId, '/Root/Devices', 2) WITH (GroupDeviceId INT, TypeId INT);
IF (@Validate = 1)
    BEGIN
        SET LOCK_TIMEOUT 1800;
        BEGIN TRY
            BEGIN TRANSACTION T1;
            DELETE dbo.AdminDevice
            WHERE  UserId = @UserId;
            INSERT INTO [dbo].[AdminDevice] ([UserId], [DeviceId], [DeviceGroupId])
            SELECT DISTINCT @UserId,
                            CASE WHEN TypeId = 0 THEN GroupDeviceId WHEN TypeId = 1 THEN NULL END,
                            CASE WHEN TypeId = 0 THEN NULL WHEN TypeId = 1 THEN GroupDeviceId END
            FROM   #Devices;
            COMMIT TRANSACTION T1;
        END TRY
        BEGIN CATCH
            IF (@@TRANCOUNT > 0)
                BEGIN
                    ROLLBACK TRANSACTION T1;
                    SET @Validate = 0;
                    SET @Message = N' خطا' + ERROR_MESSAGE();
                    SET @Code =N'400';
                END
        END CATCH
    END
SELECT TOP (1) @Message AS [Message],
               @Validate AS [Validate],
               @Code AS Code,
               @UserId AS Id;
DROP TABLE #AdminDevice;
DROP TABLE #Devices;
GO
