ALTER PROCEDURE [dbo].[InsertReservationsBatch]
@reservationTable ReservationTable readonly
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ثبت با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0
    BEGIN TRY
        BEGIN TRANSACTION T1
        BEGIN

						;with 
						Src as(
						select [Id] ,[UserId] ,[FoodId] ,[MealId] ,[RestaurantId] ,[Count] ,[ReserveTime]
						 from @reservationTable rt )
						--, a as(select * from [Log] l )


						MERGE [Rst].[Reservation] AS T
						USING (select  [Id] ,[UserId] ,[FoodId] ,[MealId] ,[RestaurantId] ,[Count] ,[ReserveTime] from Src
						) as S
						ON (T.UserId = S.UserId AND T.FoodId = S.FoodId AND T.MealId = S.MealId AND S.RestaurantId = T.RestaurantId AND S.[Count] = T.[Count]
						)
						WHEN NOT MATCHED BY Target
						THEN INSERT([UserId] ,[FoodId] ,[MealId] ,[RestaurantId] ,[Count] ,[ReserveTime], [TimeStamp]) 
						VALUES ([UserId] ,[FoodId] ,[MealId] ,[RestaurantId] ,[Count] ,[ReserveTime], GETDATE());
						;
						--WHEN NOT MATCHED BY Source 
						--THEN DELETE ;


					SELECT @Code = cast(@@ROWCOUNT as nvarchar);

        END
        --SET @Code = SCOPE_IDENTITY()
        COMMIT TRANSACTION T1
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN

                ROLLBACK TRANSACTION T1
				--select ERROR_MESSAGE()
                SET @Validate = 0
                SET @Message = N'ثبت انجام نشد'
            END
    END CATCH
    SELECT @Message AS [Message],
           @Code AS Id,
           @Validate AS Validate
END