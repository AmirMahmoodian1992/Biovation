create PROCEDURE [dbo].[InsertLicensePlate]
@LicensePlate NVARCHAR (50),
@IsActive bit = null,
@StartDate DATE = null,
@EndDate DATE = null,
@StartTime TIME = null,
@EndTime TIME = null,
@ResultCode INT=NULL OUTPUT
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ثبت با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0, @LicensePlateId AS INT = 0;
    BEGIN TRY
        BEGIN TRANSACTION T1;
        BEGIN
            DECLARE @EnglishLicensePlate NVARCHAR (50)
            SELECT @EnglishLicensePlate = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE
                                            (@LicensePlate,N'۰',N'0'),N'۱',N'1'),N'۲',N'2'),N'۳',N'3'),N'۴',N'4')
                                                            ,N'۵',N'5'),N'۶',N'6'),N'۷',N'7'),N'۸',N'8'),N'۹',N'9')

            IF NOT EXISTS (SELECT LicensePlate
                           FROM   [LicensePlate] AS LP
                           WHERE  (@LicensePlate NOT LIKE N'[۰-۹][۰-۹][آ-ی][۰-۹][۰-۹][۰-۹][۰-۹][۰-۹]' AND [LP].[LicensePlate] = @LicensePlate) OR (@LicensePlate  LIKE N'[۰-۹][۰-۹][آ-ی][۰-۹][۰-۹][۰-۹][۰-۹][۰-۹]' AND [LP].[FirstPart] = SUBSTRING(@EnglishLicensePlate,1,2) AND [LP].[SecondPart] = SUBSTRING(@LicensePlate,3,1) AND [LP].[ThirdPart] = SUBSTRING(@EnglishLicensePlate,4,3) AND [LP].[FourthPart] = SUBSTRING(@EnglishLicensePlate,7,2)))
                BEGIN
                    INSERT  INTO [dbo].[Entity] ([Typeid])
                    VALUES                     (2);
                    SET @LicensePlateId = SCOPE_IDENTITY();
                    SET @ResultCode = @LicensePlateId;
                    SET @Code = @ResultCode;
					IF @LicensePlate LIKE N'[۰-۹][۰-۹][آ-ی][۰-۹][۰-۹][۰-۹][۰-۹][۰-۹]'
					--IF LEN(@LicensePlate) = 8
						BEGIN
							SET @Message = N'تشخیص ';
							INSERT  INTO [dbo].[LicensePlate] ([Entityid], [LicensePlate],[FirstPart], [SecondPart], [ThirdPart], [FourthPart], [IsActive], [StartDate], [EndDate], [StartTime], [EndTime])
							VALUES                           (@LicensePlateId, @LicensePlate,SUBSTRING(@EnglishLicensePlate,1,2),SUBSTRING(@LicensePlate,3,1), SUBSTRING(@EnglishLicensePlate,4,3), SUBSTRING(@EnglishLicensePlate,7,2), @IsActive, @StartDate, @EndDate, @StartTime, @EndTime);
						END
					ELSE
						BEGIN
							SET @Message = N'تشخیص نداد ';
							INSERT  INTO [dbo].[LicensePlate] ([Entityid], [LicensePlate], [IsActive], [StartDate], [EndDate], [StartTime], [EndTime])
		                    VALUES                           (@LicensePlateId, @LicensePlate, @IsActive, @StartDate, @EndDate, @StartTime, @EndTime);

						END

                END
            ELSE
                BEGIN
                    SET @ResultCode = (SELECT Entityid
                                       FROM   [LicensePlate]
                                       WHERE  [LicensePlate] = @LicensePlate);
                    SET @Code = @ResultCode;
                    SET @Message = N'پلاک قبلا ثبت شده است ';
                END
        END
        COMMIT TRANSACTION T1;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1;
                SET @Validate = 0;
                SET @Message = N'ثبت انجام نشد';
            END
    END CATCH

    	SELECT @Message AS [Message],
           @Code AS Id,
           @Code AS Code,
           @Validate AS Validate;
END