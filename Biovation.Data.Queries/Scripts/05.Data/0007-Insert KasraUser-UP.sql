
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

IF NOT EXISTS (SELECT * FROM [dbo].[User] WHERE [Id] = 123456789)
INSERT INTO [dbo].[User]
           ([Id]
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



