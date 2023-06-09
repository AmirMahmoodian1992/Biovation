Create PROCEDURE [dbo].[ModifyUser]
@Id INT,
@UserCode BIGINT,
@UniqueId BIGINT = NULL,
@FName NVARCHAR (200) = NULL, 
@LName NVARCHAR (200) = NULL, 
@SDate DATETIME,
@EDate DATETIME,
@Active BIT, 
@AdminLevel INT, 
@Password NVARCHAR (200) = NULL,
@Image NVARCHAR (MAX) = NULL, 
@AuthMode INT = NULL,
@Email NVARCHAR (100) = NULL, 
@TelNumber NVARCHAR (100) = NULL, 
@Type INT, 
@EntityId INT=0, 
@IsAdmin BIT
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @Code AS INT = 0;
    BEGIN TRY
        BEGIN TRANSACTION T1;
        BEGIN
            IF @Id = 0
                BEGIN
                    IF NOT EXISTS (SELECT [Code]
                                   FROM   [dbo].[User]
                                   WHERE  [Code] = @UserCode)
                        BEGIN
                            INSERT  INTO [dbo].[User] ([Code], [UniqueId], [FirstName], [SurName], [Password], [AdminLevel], [IsActive], [AuthMode], [EndDate], [StartDate], [RegisterDate], [Image], [Email], [TelNumber], [Type], EntityId, [IsAdmin])
                            VALUES                   (@UserCode, CASE WHEN @UniqueId = 0 THEN -@UserCode ELSE ISNULL(@UniqueId, -@UserCode) END, @FName, @LName, @Password, @AdminLevel, @Active, @AuthMode, @EDate, @SDate, GETDATE(), @Image, @Email, @TelNumber, @Type, @EntityId, @IsAdmin);
                        
							 SELECT @Id = SCOPE_IDENTITY();
                             SET @Message = N'ایجاد با موفقیت انجام شد';
                             SET @Code = 200;
						END
                    ELSE
                        BEGIN
                            ROLLBACK TRANSACTION T1;
                            SET @Validate = 0;
                            SET @Message = N'کاربر با شماره کاربری تکراری';
                        END
                END
            ELSE
                BEGIN
                    IF EXISTS (SELECT [Id]
                                   FROM   [dbo].[User]
                                   WHERE  [Id] = @Id)
                        BEGIN
                            IF EXISTS (SELECT [Id]
                               FROM   [dbo].[User]
                               WHERE  [Id] <> @Id AND [Code] = @UserCode AND ([UniqueId] = ISNULL(@UniqueId, -@Code) OR @UniqueId = 0))
							BEGIN
								ROLLBACK TRANSACTION T1;
								SET @Validate = 0;
								SET @Message = N'کاربر با شماره کاربری تکراری';
							END
							ELSE
							BEGIN
								UPDATE [dbo].[User]
								SET    [FirstName]  = @FName,
									   [SurName]    = @LName,
									   [AdminLevel] = @AdminLevel,
									   [IsActive]   = @Active,
									   [AuthMode]   = @AuthMode,
									   [EndDate]    = @EDate,
									   [StartDate]  = @SDate,
									   [Image]      = @Image,
									   [Email]      = @Email,
									   [TelNumber]  = @TelNumber,
									   [Type]       = @type,
									   EntityId     = @EntityId,
									   [IsAdmin]    = @IsAdmin,
									   [Code]       = @UserCode,
									   [UniqueId]   = CASE WHEN @UniqueId = 0 THEN -@UserCode ELSE ISNULL(@UniqueId, -@UserCode) END
								WHERE  Id = @Id;
							END
                        END
                    ELSE
                        BEGIN
                            ROLLBACK TRANSACTION T1;
                            SET @Validate = 0;
                            SET @Message = N'کاربر یافت نشد';
                        END
                END
        END
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
           @Id AS Id,
           @Validate AS Validate;
END