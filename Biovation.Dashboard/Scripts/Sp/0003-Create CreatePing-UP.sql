CREATE PROCEDURE [dbo].[InsertPing]
@name nvarchar(25),
@hostAddress nvarchar(25),
@DestinationAddress  nvarchar(25),
@roundTripTime BIGINT,
@Data nvarchar(Max)
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ثبت با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0
	DECLARE @CreateCommand varchar(MAX);
    BEGIN TRY
        BEGIN TRANSACTION T1
        BEGIN
			SET @CreateCommand = 'INSERT  INTO [dbo]. ' + @name + ' ([hostAddress], [DestinationAddress], [ttl], [roundTripTime], [Data], [Timestamp])
            VALUES                (@hostAddress, @DestinationAddress, @roundTripTime, @Data, GETDATE())'

            EXEC (@CreateCommand)
            --INSERT  INTO [dbo].[Ping] ([hostAddress], [DestinationAddress], [ttl], [roundTripTime], [status], [Timestamp])
            --VALUES                  (@hostAddress, @DestinationAddress, @ttl, @roundTripTime, @status, GETDATE())
            
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