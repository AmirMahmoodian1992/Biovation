
CREATE PROCEDURE [dbo].[ModifyReservation]
@Id int,
@UserId int,
@FoodId int,
@MealId int,
@RestaurantId int,
@Count int,
@ReserveTime datetime,
@TimeStamp datetime

AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0
    BEGIN TRY
        BEGIN TRANSACTION T1
        BEGIN
            IF NOT EXISTS (SELECT Id
                           FROM   [Rst].[Reservation]
                           WHERE  Id = @Id)
                BEGIN
                    INSERT  INTO [Rst].[Reservation] ([UserId] ,[FoodId] ,[MealId] ,[RestaurantId] ,[Count] ,[ReserveTime] ,[TimeStamp])
					VALUES (@UserId ,@FoodId ,@MealId ,@RestaurantId ,@Count ,@ReserveTime ,@TimeStamp)
					SELECT @Id = SCOPE_IDENTITY()
                END
            ELSE
			  BEGIN
                UPDATE [Rst].[Reservation]
                SET    [UserId] = @UserId
					  ,[FoodId] = @FoodId
					  ,[MealId] = @MealId
					  ,[RestaurantId] = @RestaurantId
					  ,[Count] = @Count
					  ,[ReserveTime] = @ReserveTime
					  ,[TimeStamp] = @TimeStamp
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