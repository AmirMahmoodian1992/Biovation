
CREATE PROCEDURE [dbo].[ModifyUserGroupMember]
@UserGroupMember NVARCHAR (MAX), @UserGroupId INT
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0;
    BEGIN TRY
        BEGIN TRANSACTION T1;
        DELETE FROM [dbo].[UserGroupMember] WHERE [GroupId] = @UserGroupId;
        INSERT INTO [dbo].[UserGroupMember]
			([UserId]
			,[GroupId]
			,[UserType]
			,[UserTypeTitle])
        SELECT  u.[Id] AS [UserId],
                @UserGroupId AS [GroupId],
                '1' AS [UserType],
                '' AS [UserTypeTitle]
                FROM OPENJSON( @UserGroupMember, '$' ) 
				WITH ([UserCode] int '$.UserCode') AS ugm INNER JOIN [dbo].[User] AS u ON [ugm].[UserCode] = [u].[Code]
--        DELETE [dbo].[UserGroupMember]
--        WHERE  GroupId = @UserGroupId;
--        DECLARE @Handle AS INT;
--        DECLARE @Xml AS NVARCHAR (1000);
----        SET @Xml = N'<Root><UserGroupMember>
----  <InducteeTypeId>245</InducteeTypeId>
----  <InducteeId>555</InducteeId>
----  <InducteeName>Pa1</InducteeName>
----  <IsNotInculded>1</IsNotInculded>
----  <UserGroupIdId>15</UserGroupIdId>
----</UserGroupMember><UserGroupMember>
----  <InducteeTypeId>245</InducteeTypeId>
----  <InducteeId>555</InducteeId>
----  <InducteeName>Pa1</InducteeName>
----  <IsNotInculded>1</IsNotInculded>
----  <UserGroupIdId>15</UserGroupIdId>
----</UserGroupMember><UserGroupMember>
----  <InducteeTypeId>245</InducteeTypeId>
----  <InducteeId>555</InducteeId>
----  <InducteeName>Pa1</InducteeName>
----  <IsNotInculded>1</IsNotInculded>
----  <UserGroupIdId>15</UserGroupIdId>
----</UserGroupMember></Root>';
--        DECLARE @UserId AS INT, @GroupId AS INT, @UserType AS NVARCHAR (10), @UserTypeTitle AS NVARCHAR (50);
--        EXECUTE sys.sp_xml_preparedocument @Handle OUTPUT, @UserGroupMember;
--        BEGIN
--            INSERT INTO [dbo].[UserGroupMember]
--					   ([UserId]
--					   ,[GroupId]
--					   ,[UserType]
--					   ,[UserTypeTitle])
--            SELECT UserId,
--                   GroupId,
--                   UserType,
--                   UserTypeTitle
--            FROM   OPENXML (@Handle, '/Root/UserGroupMember', 2) WITH (UserId INT, GroupId INT, UserType NVARCHAR (10), UserTypeTitle NVARCHAR (50));
--        END
--        EXECUTE sys.sp_xml_removedocument @Handle;
        SET @Code = SCOPE_IDENTITY();
        COMMIT TRANSACTION T1;
    END TRY
    BEGIN CATCH
        IF (@@TRANCOUNT > 0)
            BEGIN
                ROLLBACK TRANSACTION T1;
                SET @Validate = 0;
                SET @Message = N'ایجاد انجام نشد';
            END
    END CATCH
    SELECT @Message AS [Message],
           @Code AS Id,
           @Validate AS Validate;
END
GO
