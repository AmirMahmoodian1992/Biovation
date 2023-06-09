
CREATE PROCEDURE [dbo].[ModifyUserGroup]
@Id INT, @Name NVARCHAR (100), @AccessGroup NVARCHAR (200), @Description NVARCHAR (200), @Users NVARCHAR(MAX) = ''
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS NVARCHAR (15) = N'200';
    BEGIN TRY
        BEGIN TRANSACTION T1;
        BEGIN
            IF NOT EXISTS (SELECT ID
                           FROM   [dbo].[UserGroup]
                           WHERE  ID = @Id)
                BEGIN
                    INSERT  INTO [dbo].[UserGroup] ([Name], [AccessGroup], [Description])
                    VALUES                        (@Name, @AccessGroup, @Description);
					 SET @Code = SCOPE_IDENTITY();

                     INSERT INTO [dbo].[UserGroupMember]
					   ([UserId]
					   ,[GroupId]
					   ,[UserType]
					   ,[UserTypeTitle])
                    SELECT  u.[Id] AS [UserId],
                            @Id AS [GroupId],
                            '1' AS [UserType],
                            '' AS [UserTypeTitle]
                            FROM OPENJSON( @Users, '$' ) 
				            WITH ([UserCode] int '$.UserCode') AS ugm INNER JOIN [dbo].[User] AS u ON [ugm].[UserCode] = [u].[Code]
                END
            ELSE
				BEGIN
					UPDATE [dbo].[UserGroup]
					SET    [Name]        = @Name,
						   [AccessGroup] = @AccessGroup,
						   [Description] = @Description
					WHERE  Id = @Id;
					SET @Code = @Id;

       --             DELETE FROM [dbo].[UserGroupMember] WHERE [GroupId] = @Id;
       --             INSERT INTO [dbo].[UserGroupMember]
					  -- ([UserId]
					  -- ,[GroupId]
					  -- ,[UserType]
					  -- ,[UserTypeTitle])
       --             SELECT  UserId,
       --                     @Code AS [GroupId],
       --                     '1' AS [UserType],
       --                     '' AS [UserTypeTitle]
       --                     FROM OPENJSON( @Users, '$' ) 
							--WITH ([UserId] int '$.Id')
					SET @Message = N'ویرایش با موفقیت انجام شد'
				END
        END
       
        COMMIT TRANSACTION T1;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1;
                SET @Validate = 0;
                SET @Code = 400;
                SET @Message = N'ایجاد انجام نشد';
            END
    END CATCH
    SELECT @Message AS [Message],
           @Code AS Id,
           @Code AS Code,
           @Validate AS Validate;
END
GO