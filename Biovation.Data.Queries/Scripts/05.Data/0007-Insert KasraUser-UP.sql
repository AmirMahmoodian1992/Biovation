
IF NOT EXISTS (SELECT * FROM [dbo].[Entity] WHERE [Id] = 1)
INSERT INTO [dbo].[Entity]
           ([TypeId]
           ,[Active]
           ,[StartDate]
           ,[EndDate]
           ,[RegisterType])
     VALUES
           (1
           ,1
           ,getdate()
           ,getdate()
           ,1)

IF NOT EXISTS (SELECT * FROM [dbo].[User] WHERE [UniqueId] = 123456789)
BEGIN
	SET IDENTITY_INSERT dbo.[User] ON

    INSERT INTO [dbo].[User]
               ([Id]
               ,[UniqueId]
		       ,[Code]
               ,[FirstName]
               ,[SurName]
               ,[UserName]
               ,[StartDate]
               ,[EndDate]
               ,[RegisterDate]
               ,[IsActive]
               ,[AdminLevel]
               ,[Password]
               ,[PasswordBytes]
               ,[Image]
               ,[AuthMode]
               ,[EntityId]
               ,[Email]
               ,[TelNumber]
               ,[Type]
               ,[IsAdmin]
               ,[IsMasterAdmin])
         VALUES
               (123456789
               ,123456789
               ,123456789
               ,'Admin'
               ,'Administrator'
               ,'Admin'
               ,getdate()
               ,getdate()
               ,getdate()
               ,1
               ,0
               ,NULL
               ,NULL
               ,NULL
               ,NULL
               ,1
               ,NULL
               ,NULL
               ,1
               ,0
               ,1)
    
	SET IDENTITY_INSERT dbo.[User] OFF
END


IF NOT EXISTS (SELECT * FROM [dbo].[User] WHERE [UniqueId] = 987654321)
BEGIN
    SET IDENTITY_INSERT dbo.[User] ON
    INSERT INTO [dbo].[User]
               ([Id]
               ,[UniqueId]
		       ,[Code]
               ,[FirstName]
               ,[SurName]
               ,[UserName]
               ,[StartDate]
               ,[EndDate]
               ,[RegisterDate]
               ,[IsActive]
               ,[AdminLevel]
               ,[Password]
               ,[PasswordBytes]
               ,[Image]
               ,[AuthMode]
               ,[EntityId]
               ,[Email]
               ,[TelNumber]
               ,[Type]
               ,[IsAdmin]
               ,[IsMasterAdmin])
         VALUES
               (987654321
               ,987654321
               ,987654321
               ,'System'
               ,'Administrator'
               ,'SystemAdmin'
               ,getdate()
               ,getdate()
               ,getdate()
               ,1
               ,0
               ,NULL
               ,NULL
               ,NULL
               ,NULL
               ,1
               ,NULL
               ,NULL
               ,1
               ,0
               ,1)
END

BEGIN TRY
    DECLARE @maxIdValue int
    SELECT @maxIdValue = MAX(Id) from [User] Where Id < 123456789
    DBCC CHECKIDENT ('user', reseed, @maxIdValue);
END TRY
BEGIN CATCH
END CATCH