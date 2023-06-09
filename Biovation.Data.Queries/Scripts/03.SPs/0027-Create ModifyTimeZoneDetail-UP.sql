
CREATE PROCEDURE [dbo].[ModifyTimeZoneDetail]
@Id INT, @TimeZoneId int, @DayNumber int, @FromTime time(7), @ToTime time(7)
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (15) = N'201';
    BEGIN TRY
        BEGIN TRANSACTION T1;
        BEGIN
            IF NOT EXISTS (SELECT ID
                           FROM   [dbo].[TimeZoneDetail]
                           WHERE  ID = @Id)
                BEGIN
                    INSERT INTO [dbo].[TimeZoneDetail]
									   ([TimeZoneId]
									   ,[DayNumber]
									   ,[FromTime]
									   ,[ToTime])
									VALUES                  
									 (@TimeZoneId, @DayNumber, @FromTime, @ToTime);
                SET @Message = N'ایجاد با موفقیت انجام شد';
                SET @Code = 200;
               END
               
            ELSE
               UPDATE [dbo].[TimeZoneDetail]
				   SET [TimeZoneId] = @TimeZoneId
					  ,[DayNumber] = @DayNumber
					  ,[FromTime] = @FromTime
					  ,[ToTime] = @ToTime
                WHERE  Id = @Id;

				SET @Message = N'ویرایش با موفقیت انجام شد';
                SET @Code = 200;
        END
        SET @Code = SCOPE_IDENTITY();
        COMMIT TRANSACTION T1;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1;
                SET @Validate = 0;
                SET @Message = N'ایجاد انجام نشد';
                SET @Code = 400;
            END
    END CATCH
    SELECT @Message AS [Message],
           @Code AS Id,
           @Code AS Code,
           @Validate AS Validate;
END
GO
