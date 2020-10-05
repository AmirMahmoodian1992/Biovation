ALter PROCEDURE [dbo].[CreatePing]
@TableName nvarchar(50),
@DbName nvarchar(50)
AS
BEGIN
DECLARE @Message AS NVARCHAR (200) = N'ثبت با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0
DECLARE @CreateCommand varchar(MAX);
    BEGIN TRY
        BEGIN TRANSACTION T1
        BEGIN

            SET @CreateCommand = 'Create table ' + @DbName + ' .dbo. ' + @TableName + ' ([hostAddress] [nvarchar](25) NOT NULL,
            [DestinationAddress] [nvarchar](25) NOT NULL,
            [RoundTripTime] [Bigint] NOT NULL,
            [Timestamp] [datetime] NOT NULL,
            [Data] [nvarchar](MAX) NOT NULL)'

            EXEC (@CreateCommand)
         END
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