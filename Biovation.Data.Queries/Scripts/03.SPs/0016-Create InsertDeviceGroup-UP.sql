
CREATE PROCEDURE [dbo].[InsertDeviceGroup]
@Id INT, @Name NVARCHAR (100), @Description NVARCHAR (200)='', @Devices NVARCHAR(MAX) = ''
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0
    BEGIN TRY
        BEGIN TRANSACTION T1
        BEGIN
            SET @Code = @Id
            IF NOT EXISTS (SELECT *
                           FROM   [dbo].[DeviceGroup]
                           WHERE  Name = @Name)
                BEGIN
                    IF @Id = 0
                    BEGIN
                        INSERT  INTO [dbo].[DeviceGroup] ([Name], [Description])
                        VALUES                          (@Name, @Description)

					    SET @Code = SCOPE_IDENTITY()
                    END

                    INSERT INTO [dbo].[DeviceGroupMember] ([DeviceId], [GroupId])
                    SELECT DeviceId, @Code AS [GroupId]
						   FROM OPENJSON( @Devices, '$' ) 
							WITH ([DeviceId] NVARCHAR(25) '$.DeviceId')
                END
            ELSE
                BEGIN
					IF @Id <> 0
					BEGIN
                        UPDATE [dbo].[DeviceGroup]
                        SET    [Name]        = @Name,
                               [Description] = @Description
                        WHERE  Id = @Id

						DELETE [dbo].DeviceGroupMember
						WHERE  GroupId = @Id;

                        INSERT INTO [dbo].[DeviceGroupMember] ([DeviceId], [GroupId])
						SELECT DeviceId, @Code AS [GroupId]
						   FROM OPENJSON( @Devices, '$' ) 
							WITH ([DeviceId] NVARCHAR(25) '$.DeviceId')

						SET @Validate = 1
						SET @Message = N'ویرایش با موفقیت انجام شد'
					END
					ELSE
					BEGIN
						SET @Validate = 0
						SET @Message = N'نام گروه تکراری است'
					END
                END
        END
        IF @Id = 0
            SET @Code = SCOPE_IDENTITY()
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
           @Code AS Id,
           @Validate AS Validate;
END
