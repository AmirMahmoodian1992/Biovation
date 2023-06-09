
CREATE PROCEDURE [dbo].[SelectGroupsOfDevice]
@DeviceId INT
AS
BEGIN
    SELECT [DevGroup].[id],
           [Name],
           [Description]
    FROM   [dbo].[DeviceGroup] AS DevGroup
           INNER JOIN
           [dbo].[DeviceGroupMember] AS DGM
           ON DGM.[GroupId] = DevGroup.[Id]
    WHERE  DGM.[DeviceId] = @DeviceId;
END
GO
