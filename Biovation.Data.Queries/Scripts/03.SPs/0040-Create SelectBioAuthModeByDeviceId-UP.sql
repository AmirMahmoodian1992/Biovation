
CREATE PROCEDURE [dbo].[SelectBioAuthModeByDeviceId]
@DeviceId int,
@AuthMode int
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'عملیات با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (15) = N'200';
	SELECT a.Id AS Id,a.BioCode AS BioCode_Code, a.BrandId AS BrandId,a.AuthMode AS AuthMode, a.BioTitle AS BioTitle ,@Message AS e_Message,
           @Validate AS e_Validate,
           @Code AS e_Code 

	FROM [AuthModeMap] a INNER JOIN
							[DeviceBrand] B ON A.BrandId = B.ID INNER JOIN
							[DeviceModel] M ON B.ID = M.BrandID INNER JOIN
							[Device] D ON D.DeviceModelID = M.ID
							WHERE D.ID = @DeviceId and a.AuthMode = @AuthMode

END
GO
