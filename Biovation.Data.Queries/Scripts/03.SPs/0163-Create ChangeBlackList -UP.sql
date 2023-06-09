
Create PROCEDURE [dbo].[ChangeBlackList] @Id int,
@UserId int, @DeviceId int, @StartDate datetime,
@EndDate datetime, @IsDeleted bit, @Description nvarchar(200) = ''
AS
BEGIN
   DECLARE @Message AS nvarchar(200) = N'ایجاد/ویرایش با موفقیت انجام گرفت',
          @Validate AS int = 1,
          @CodeINSERT AS int = 200
  DECLARE @Result_Delete AS TABLE (
    Message nvarchar(200),
    Id int,
    Validate int
  )
  DECLARE @Result_Insert AS TABLE (
    Message nvarchar(200),
    Validate int,
	Id int
  )


  BEGIN TRY
    INSERT @Result_Delete EXEC [dbo].[DeleteBlackList] @Id = @Id
    IF NOT EXISTS (SELECT
        *
      FROM @Result_Delete
      WHERE Validate = 0)
    BEGIN

      INSERT @Result_Insert EXEC [dbo].[InsertBlackList] @UserId = @UserId,
                                                         @DeviceId = @DeviceId,
                                                         @StartDate = @StartDate,
                                                         @EndDate = @EndDate,
                                                         @IsDeleted = @IsDeleted,
                                                         @Description = @Description

      IF EXISTS (SELECT
          *
        FROM @Result_Insert
        WHERE Validate = 0)
      BEGIN
        SET @Validate = 0
        SET @Message = (SELECT
          Message
        FROM @Result_Insert)
        ROLLBACK TRANSACTION T1
      END


    END

    ELSE
    BEGIN
      SET @Validate = 0
      SET @Message = (SELECT
        Message
      FROM @Result_Delete)
      ROLLBACK TRANSACTION T1
    END

  END TRY
  BEGIN CATCH
    IF (@@TRANCOUNT > 0)
    BEGIN
      ROLLBACK TRANSACTION T1
      SET @Validate = 0
      SET @Message = N'ایجاد/ویرایش انجام نشد'
      SET @CodeINSERT = 400
    END
  END CATCH
  SELECT
    @Message [Message],
    @CodeINSERT AS Id,
    @Validate AS Validate,
    @CodeINSERT AS Code
END