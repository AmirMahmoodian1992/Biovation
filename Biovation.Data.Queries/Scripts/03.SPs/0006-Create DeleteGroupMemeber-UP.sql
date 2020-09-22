
CREATE PROCEDURE [dbo].[DeleteGroupMemeber]
@DeviceId INT, @GroupId INT
AS
BEGIN
    DELETE [dbo].[DeviceGroupMember]
    WHERE  DeviceId = @DeviceId
           AND GroupId = @GroupId;
END
GO
