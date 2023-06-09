CREATE PROCEDURE [dbo].[SelectLicensePlateByFilter]
@LicensePlate NVARCHAR (50) = NULL,
@LicensePlateId INT = NULL
AS
DECLARE @Message AS NVARCHAR (200) = N' درخواست با موفقیت انجام گرفت', @Validate AS INT = 1,  @Code AS NVARCHAR (15) = N'200';
BEGIN
    DECLARE @EnglishLicensePlate NVARCHAR (50)
    SELECT @EnglishLicensePlate = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE
                                            (@LicensePlate,N'۰',N'0'),N'۱',N'1'),N'۲',N'2'),N'۳',N'3'),N'۴',N'4')
                                                            ,N'۵',N'5'),N'۶',N'6'),N'۷',N'7'),N'۸',N'8'),N'۹',N'9')

    SELECT [LP].[Entityid],
           [LP].[LicensePlate] AS LicensePlateNumber,
           [LP].[IsActive],
           [LP].[StartDate],
           [LP].[EndDate],
           [LP].[StartTime],
           [LP].[EndTime],
		   @Message AS e_Message,
           @Validate AS e_Validate,
           @Code AS e_Code
    FROM   [dbo].[LicensePlate] AS [LP]
    WHERE  (ISNULL(@LicensePlate, '0') = '0'
            OR (@LicensePlate NOT LIKE N'[۰-۹][۰-۹][آ-ی][۰-۹][۰-۹][۰-۹][۰-۹][۰-۹]'
                AND [LP].[LicensePlate] = @LicensePlate)
            OR (@LicensePlate LIKE N'[۰-۹][۰-۹][آ-ی][۰-۹][۰-۹][۰-۹][۰-۹][۰-۹]'
                AND [LP].[FirstPart] = SUBSTRING(@EnglishLicensePlate, 1, 2)
                AND [LP].[SecondPart] = SUBSTRING(@LicensePlate, 3, 1)
                AND [LP].[ThirdPart] = SUBSTRING(@EnglishLicensePlate, 4, 3)
                AND [LP].[FourthPart] = SUBSTRING(@EnglishLicensePlate, 7, 2)))
           AND (ISNULL(@LicensePlateId, 0) = 0
                OR [LP].[Entityid] = @LicensePlateId);
END