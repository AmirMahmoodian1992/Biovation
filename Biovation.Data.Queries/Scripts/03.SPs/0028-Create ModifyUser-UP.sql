CREATE PROCEDURE [dbo].[ModifyUser]
@Id INT,
@FName NVARCHAR (200)=NULL, 
@LName NVARCHAR (200)=NULL, 
@SDate SMALLDATETIME,
@EDate SMALLDATETIME,
@Active BIT, 
@AdminLevel INT, 
@Password NVARCHAR (200)=NULL,
@Image VARBINARY (MAX)=NULL, 
@AuthMode INT=NULL,
@Email NVARCHAR (100)=NULL, 
@TelNumber NVARCHAR (100)=NULL, 
@Type INT, 
@EntityId INT=0, 
@IsAdmin BIT
AS
BEGIN
    DECLARE @Message AS NVARCHAR (200) = N'ایجاد با موفقیت انجام گرفت', @Validate AS INT = 1, @Code  AS nvarchar(5) = N'201'
    BEGIN TRY
        BEGIN TRANSACTION T1
        BEGIN
            IF NOT EXISTS (SELECT ID
                           FROM   [dbo].[User]
                           WHERE  ID = @Id)
                BEGIN
                    INSERT  INTO [dbo].[User] ([ID], [FirstName], [SurName], [Password], [AdminLevel], [IsActive], [AuthMode], [EndDate], [StartDate], [RegisterDate], [Image], [Email], [TelNumber], [Type], EntityId, [IsAdmin])
                    VALUES                   (@Id, @FName, @LName, @Password, @AdminLevel, @Active, @AuthMode, @EDate, @SDate, GETDATE(), @Image, @Email, @TelNumber, @Type, @EntityId, @IsAdmin)
					--SET @Code = SCOPE_IDENTITY()
                END
            ELSE
			  BEGIN
                UPDATE [dbo].[User]
                SET    [FirstName]  = @FName,
                       [SurName]    = @LName,
                      -- [Password]   = @Password,
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
                       [IsAdmin]    = @IsAdmin
                WHERE  Id = @Id

				--SET @Code = @Id
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
                SET  @Code  = N'400'
            END
    END CATCH
    SELECT @Message AS [Message],
           @Id AS Id,
           @Validate AS Validate,
           @Code AS Code
END