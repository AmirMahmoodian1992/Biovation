Create PROCEDURE [dbo].[InsertManualPlateDetectionLog]
--declare
@LicensePlateId INT, @DetectorId INT, @UserId BIGINT, @ParentLogId BIGINT, @EventId INT, @LicensePlateNumber NVARCHAR (20), @LogDateTime DATETIME, @Ticks BIGINT, @DetectionPrecision TINYINT, @FullImage VARBINARY (MAX)=NULL, @PlateImage VARBINARY (MAX)=NULL, @InOrOut TINYINT=0
AS
--select @LicensePlateId =0, @DetectorId =3, @UserId =123456789, @ParentLogId =0, @EventId =16004, @LicensePlateNumber =N'22ب56765', @LogDateTime ='2021-08-07 15:43:10.753', @Ticks =19, @DetectionPrecision =68, @FullImage =NULL, @PlateImage =NULL, @InOrOut =2
BEGIN

--insert into temp_InsertManualPlateDetectionLog([LicensePlateId], [DetectorId], [UserId], [ParentLogId], [EventId], [LicensePlateNumber], [LogDateTime], [Ticks], [DetectionPrecision], [FullImage], [PlateImage], [InOrOut])
--select @LicensePlateId , @DetectorId , @UserId , @ParentLogId , @EventId , @LicensePlateNumber , @LogDateTime , @Ticks , @DetectionPrecision , @FullImage , @PlateImage , @InOrOut 

--select top 1 @LicensePlateId=g.LicensePlateId , @DetectorId =g.DetectorId, @UserId=g.UserId , @ParentLogId =g.ParentLogId, @EventId =g.EventId, @LicensePlateNumber=g.LicensePlateNumber , @LogDateTime=g.LogDateTime , @Ticks=g.Ticks , @DetectionPrecision =g.DetectionPrecision, @FullImage=g.FullImage , @PlateImage=g.PlateImage , @InOrOut =g.InOrOut
--select *
--from temp_InsertManualPlateDetectionLog g where g.LicensePlateNumber=N'11ب11165' and PlateImage is not null

    DECLARE @Message AS NVARCHAR (200) = N'ثبت با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0, @Logid AS INT = 0, @FarsiLicensePlate  NVARCHAR (20);
    BEGIN TRY
        BEGIN TRANSACTION T1;
        BEGIN
            IF NOT EXISTS (SELECT ID
                           FROM   [PlateDetectionLog]
                           WHERE  [DetectorId] = @DetectorId
                                  AND [EventId] = @EventId
                                  AND [LicensePlateId] = @LicensePlateid
                                  AND ([LogDateTime] = @LogDateTime
                                       OR [Ticks] = @Ticks))
                BEGIN
				 -- select @LicensePlateNumber
				  SELECT @FarsiLicensePlate = REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(@LicensePlateNumber, N'0' ,N'۰'),N'1', N'۱' ),  N'2',N'۲'),N'3' ,N'۳' ),N'4',  N'۴'), N'5' ,N'۵'),N'6' ,N'۶' ),N'7', N'۷' ), N'8' ,N'۸'), N'9' ,N'۹');


				--select @LicensePlateId,@LicensePlateNumber
                    SELECT @LicensePlateId = [EntityId]
                    FROM   [dbo].[LicensePlate]
                    WHERE  [LicensePlate] LIKE @FarsiLicensePlate;


                    IF ISNULL(@LicensePlateId, 0) = 0
                        BEGIN
						--select @FarsiLicensePlate
                            EXECUTE [dbo].[InsertLicensePlate] @LicensePlate = @FarsiLicensePlate, @IsActive = 0, @StartDate = NULL, @EndDate = NULL, @StartTime = NULL, @EndTime = NULL;
						--select 111
                        END
                    SELECT @LicensePlateId = [EntityId]
                    FROM   [dbo].[LicensePlate]
                    WHERE  [LicensePlate] LIKE @FarsiLicensePlate;
					
                    IF ISNULL(@LicensePlateId, 0) <> 0 
                        BEGIN
						if not exists(select top 1 1 from [Rly].ManualPlateDetectionLog where ParentLogId=@ParentLogId)
						begin
                            INSERT  INTO [Rly].ManualPlateDetectionLog ([DetectorId], [UserId], [ParentLogId], [EventId], [LicensePlateid], [LogDateTime], [Ticks], [DetectionPrecision], [SuccessTransfer], [TimeStamp], [InOrOut])
                            VALUES                                    (@DetectorId, @UserId, @ParentLogId, @EventId, @LicensePlateId, @LogDateTime, @Ticks, @DetectionPrecision, 0, GETDATE(), @InOrOut);
                            SET @Logid = SCOPE_IDENTITY();
                            SET @Code = @Logid;
							--قرار شد فقط در صورتی که ثبت تردد دستی جدید می باشد تصویر ذخیره گردد
                          --  EXECUTE InsertPlateDetectionLogPicture @Logid, @FullImage, @PlateImage;
						  end
						  else
						  begin
								SET @Validate = 0;
								SET @Message = N'لاگ دستی دیگری وجود دارد';
						  end
                        END
                END
        END
        COMMIT TRANSACTION T1;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
			--select ERROR_MESSAGE()
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