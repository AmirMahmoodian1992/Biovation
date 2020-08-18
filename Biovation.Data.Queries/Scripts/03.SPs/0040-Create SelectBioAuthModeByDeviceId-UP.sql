
CREATE PROCEDURE [dbo].[SelectBioAuthModeByDeviceId]
@DeviceId int,
@AuthMode int
AS
BEGIN
	SELECT a.Id ,a.BioCode , a.BrandId ,a.AuthMode , a.BioTitle FROM [AuthModeMap] a INNER JOIN
							[DeviceBrand] B ON A.BrandId = B.ID INNER JOIN
							[DeviceModel] M ON B.ID = M.BrandID INNER JOIN
							[Device] D ON D.DeviceModelID = M.ID
							WHERE D.ID = @DeviceId and a.AuthMode = @AuthMode


END
GO
