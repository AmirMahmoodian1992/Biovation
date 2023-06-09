
CREATE PROCEDURE [dbo].[DeleteUserGroupOfUser]
	@userId int,
	@userGroupId int,
	@userTypeId int = 1
AS
BEGIN

	DECLARE @Message AS NVARCHAR (200) = N'ثبت با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (15) = N'200'

	BEGIN TRY
        BEGIN TRANSACTION T1
        
            DELETE FROM [dbo].[UserGroupMember]
			WHERE [UserId] = @userId AND [GroupId] = @userGroupId AND [UserType] = @userTypeId
           
            SET @Code = @userId
        COMMIT TRANSACTION T1
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1
                SET @Validate = 0
                SET @Code = 400
                SET @Message = N'ایجاد انجام نشد'
            END
    END CATCH
    SELECT @Message AS [Message],
           @Code AS Id,
           @Code AS Code,
           @Validate AS Validate

END
GO
