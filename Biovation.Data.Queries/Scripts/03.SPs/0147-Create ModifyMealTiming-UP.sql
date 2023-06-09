
CREATE PROCEDURE [dbo].[ModifyMealTiming]
@Id int,
@MealId int,
@DeviceId int,
@StartDate datetime,
@EndDate datetime,
@StartTimeInMinutes int,
@EndTimeInMinutes int

AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0
    BEGIN TRY
        BEGIN TRANSACTION T1
        BEGIN
            IF NOT EXISTS (SELECT Id
                           FROM   [Rst].[MealTiming]
                           WHERE  Id = @Id)
                BEGIN
                    INSERT  INTO [Rst].[MealTiming] ([MealId], [DeviceId], [StartDate], [EndDate], [StartTimeInMinutes], [EndTimeInMinutes]) 
					VALUES (@MealId, @DeviceId, @StartDate, @EndDate, @StartTimeInMinutes, @EndTimeInMinutes)
					SELECT @Id = SCOPE_IDENTITY()
                END
            ELSE
			  BEGIN
                UPDATE [Rst].[MealTiming]
                SET    [MealId] = @MealId
					  ,[DeviceId] = @DeviceId
					  ,[StartDate] = @StartDate
					  ,[EndDate] = @EndDate
					  ,[StartTimeInMinutes] = @StartTimeInMinutes
					  ,[EndTimeInMinutes] = @EndTimeInMinutes
                WHERE  Id = @Id
				SET @Message = N'ویرایش با موفقیت انجام گرفت'
			END
        END
        
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
           @Id AS Id,
           @Validate AS Validate
END