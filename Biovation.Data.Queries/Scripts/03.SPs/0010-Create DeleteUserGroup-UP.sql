CREATE PROCEDURE [dbo].[DeleteUserGroup]
 @Id INT
AS
BEGIN
    BEGIN TRY
		DECLARE @groupName AS NVARCHAR (20)  = (SELECT u.[Name] FROM UserGroup AS U WHERE [Id] = @Id)
        DECLARE @Message AS NVARCHAR (200) = N'گروه ' + @groupName + N' با موفقیت حذف شد',
		--N'حذف با موفقیت انجام شد',
		 @Validate AS INT = 1,
		 @Code AS INT = 0;
        BEGIN TRANSACTION T1;
        IF NOT EXISTS (SELECT *
                       FROM   AccessGroupUser
                       WHERE  UserGroupId = @Id)
            BEGIN
                DELETE [dbo].[UserGroupMember]
                WHERE  [GroupId] = @Id;
                DELETE [dbo].[UserGroup]
                WHERE  Id = @Id;
            END
        ELSE
            BEGIN
                SET @Validate = 0;
				SET @Message = N'گروه ' + @groupName + N' در کنترل دسترسی استفاده شده است'
                --SET @Message = N'این گروه در کنترل دسترسی استفاده شده است';
            END
        COMMIT TRANSACTION T1;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1;
                SET @Validate = 0;
                SET @Message = N'حذف یوزر با مشکل مواجه شد';
            END
    END CATCH
    SELECT @Message AS [Message],
           @Id AS Id,
           @Validate AS Validate;
END