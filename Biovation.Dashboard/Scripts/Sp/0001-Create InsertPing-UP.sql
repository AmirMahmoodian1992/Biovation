CREATE PROCEDURE [dbo].[InsertPing]
@hostAddress nvarchar(25),
@DestinationAddress  nvarchar(25),
@roundTripTime BIGINT,
@status nvarchar(20)
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ثبت با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0
    BEGIN TRY
        BEGIN TRANSACTION T1
        BEGIN
                INSERT  INTO [dbo].[Ping] ([hostAddress], [DestinationAddress], [ttl], [roundTripTime], [status], [Timestamp])
                VALUES                  (@hostAddress, @DestinationAddress, @ttl, @roundTripTime, @status, GETDATE())
            
        END
        SET @Code = SCOPE_IDENTITY()
        COMMIT TRANSACTION T1
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1
                SET @Validate = 0
                SET @Message = N'ثبت انجام نشد'
            END
    END CATCH
    SELECT @Message AS [Message],
           @Code AS Id,
           @Validate AS Validate
END