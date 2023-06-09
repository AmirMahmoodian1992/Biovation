
CREATE PROCEDURE [dbo].[InsertNetworkConnectionLog]
	@deviceId int,
	@UpdateDate DateTime,
	@IPAddress NVARCHAR(16)
AS
BEGIN

    DECLARE @Message AS NVARCHAR (200) = N'ثبت با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0

	BEGIN TRY
        BEGIN TRANSACTION T1
        
            INSERT INTO [dbo].[DeviceNetworkHistory]
			   ([DeviceId]
			   ,[UpdateDate]
			   ,[IPAddress])
		 VALUES
			   (@deviceId
			   ,@UpdateDate
			   ,@IPAddress)
           
            SET @Code = @deviceId
        COMMIT TRANSACTION T1
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1
                SET @Validate = 0
                SET @Message = N'ایجاد انجام نشد'
            END
    END CATCH
    SELECT @Message AS [Message],
           @Code AS Id,
           @Validate AS Validate
END
GO
