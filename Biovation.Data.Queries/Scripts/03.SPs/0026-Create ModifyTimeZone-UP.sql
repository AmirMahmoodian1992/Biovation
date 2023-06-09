
CREATE PROCEDURE [dbo].[ModifyTimeZone]
@Id INT, @Name NVARCHAR(100), @TimeZoneDetails NVARCHAR(MAX) = NULL
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (15) = N'201';
    BEGIN TRY
        BEGIN TRANSACTION T1;
        BEGIN
            IF NOT EXISTS (SELECT ID
                           FROM   [dbo].[TimeZone]
                           WHERE  ID = @Id)
                BEGIN
                    INSERT INTO [dbo].[TimeZone]
									   ([Name])
									VALUES                  
									 (@Name);

                    SET @Id = SCOPE_IDENTITY();
                    DELETE FROM [TimeZoneDetail] WHERE [TimeZoneId] = @Id

                    IF EXISTS (SELECT * FROM OPENJSON ( @TimeZoneDetails ) )
                     BEGIN
                        INSERT INTO [dbo].[TimeZoneDetail]
					        ([TimeZoneId]
					        ,[DayNumber]
					        ,[FromTime]
					        ,[ToTime])
				        SELECT @Id,
                               [DayNumber],
                               [FromTime],
                               [ToTime]
                        FROM OPENJSON (@TimeZoneDetails) WITH (DayNumber int, FromTime time(7), ToTime time(7));
                     END

                    SET @Message = N'ایجاد با موفقیت انجام شد';
                    SET @Code = N'200';
                END
            ELSE
               UPDATE [dbo].[TimeZone]
				   SET [Name] =@Name
                WHERE  Id = @Id;

                DELETE FROM [TimeZoneDetail] WHERE [TimeZoneId] = @Id

                IF EXISTS (SELECT * FROM OPENJSON ( @TimeZoneDetails ) )
                    BEGIN
                        INSERT INTO [dbo].[TimeZoneDetail]
					        ([TimeZoneId]
					        ,[DayNumber]
					        ,[FromTime]
					        ,[ToTime])
				        SELECT  @Id,
                                [DayNumber],
                                [FromTime],
                                [ToTime]
                        FROM OPENJSON (@TimeZoneDetails) WITH (DayNumber int, FromTime time(7), ToTime time(7));
                    END

				SET @Message = N'ویرایش با موفقیت انجام شد';
                SET @Code  = N'400';
        END
        --SET @Id = SCOPE_IDENTITY();
        COMMIT TRANSACTION T1;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1;
                SET @Validate = 0;
                SET @Message = N'ایجاد انجام نشد';
            END
    END CATCH
    SELECT @Message AS [Message],
           @Id AS Id,
           @Code AS Code,
           @Validate AS Validate;
END
GO
