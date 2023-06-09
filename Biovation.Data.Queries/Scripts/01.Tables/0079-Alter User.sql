IF COL_LENGTH('dbo.User', 'UniqueId') IS NULL And COL_LENGTH('dbo.User', 'Code') IS  NULL
	BEGIN 
		SELECT * INTO #tempuser
		FROM dbo.[User]

		BEGIN TRY
			BEGIN TRANSACTION T1
			IF EXISTS (select *
						from INFORMATION_SCHEMA.TABLE_CONSTRAINTS
						where CONSTRAINT_TYPE='FOREIGN KEY' AND CONSTRAINT_SCHEMA = 'dbo' AND CONSTRAINT_NAME = 'FK_AccessGroup_User')
			BEGIN	
				ALTER TABLE dbo.[AccessGroup]
				DROP CONSTRAINT  FK_AccessGroup_User
			END

			IF EXISTS	(select *
						from INFORMATION_SCHEMA.TABLE_CONSTRAINTS
						where CONSTRAINT_TYPE='FOREIGN KEY' AND CONSTRAINT_SCHEMA = 'dbo' AND CONSTRAINT_NAME = 'FK_FingerTemplate_User')
			BEGIN	
				ALTER TABLE dbo.[FingerTemplate]
				DROP CONSTRAINT  FK_FingerTemplate_User
			END

			IF EXISTS (select *
						from INFORMATION_SCHEMA.TABLE_CONSTRAINTS
						where CONSTRAINT_TYPE='FOREIGN KEY' AND CONSTRAINT_SCHEMA = 'dbo' AND CONSTRAINT_NAME = 'FK_BlackList_User')
			BEGIN	
				ALTER TABLE dbo.[BlackList]
				DROP CONSTRAINT  FK_BlackList_User
			END


			IF EXISTS (select *
						from INFORMATION_SCHEMA.TABLE_CONSTRAINTS
						where CONSTRAINT_TYPE='FOREIGN KEY' AND CONSTRAINT_SCHEMA = 'dbo' AND CONSTRAINT_NAME = 'FK_Task_User')
			BEGIN	
				ALTER TABLE dbo.[Task]
				DROP CONSTRAINT  FK_Task_User
			END

			IF EXISTS (select *
						from INFORMATION_SCHEMA.TABLE_CONSTRAINTS
						where CONSTRAINT_TYPE='FOREIGN KEY' AND CONSTRAINT_SCHEMA = 'Rst' AND CONSTRAINT_NAME = 'FK_ServeLog_User')
			BEGIN	
				ALTER TABLE Rst.[ServeLog]
				DROP CONSTRAINT  FK_ServeLog_User
			END

			IF OBJECT_ID('dbo.DF_User_RegisterDate', 'D') IS NOT NULL
			BEGIN	
				ALTER TABLE dbo.[User]
				DROP CONSTRAINT DF_User_RegisterDate
			END

			COMMIT TRANSACTION T1

			CREATE TABLE [dbo].[NewUser](
			[Id] [int] IDENTITY(1, 1) PRIMARY KEY NOT NULL,
			[UniqueId] [BIGINT] UNIQUE NOT NULL,
			[Code] [BIGINT] NOT NULL,
			[FirstName] [nvarchar](200) NULL,
			[SurName] [nvarchar](200) NULL,
			[UserName] [nvarchar](200) NULL,
			[StartDate] [smalldatetime] NOT NULL,
			[EndDate] [smalldatetime] NOT NULL,
			[RegisterDate] [smalldatetime] NOT NULL CONSTRAINT [DF_User_RegisterDate]  DEFAULT (getdate()),
			[IsActive] [bit] NOT NULL,
			[IsAdmin] [bit] NOT NULL,
			[AdminLevel] [int] NOT NULL,
			[Password] [nvarchar](max) NULL,
			[PasswordBytes] [binary](50) NULL,
			[AuthMode] [int] NULL,
			[IsMasterAdmin] [bit] NULL,
			[Image] [nvarchar](MAX) NULL,
			[EntityId] [int] NULL,
			[Email] [nvarchar](100) NULL,
			[TelNumber] [nvarchar](200) NULL,
			[Type] [int] NULL,
			[RemainingCredit] [real] NULL,
			[AllowedStockCount] [int] NULL)
		END TRY
		BEGIN CATCH
			ROLLBACK TRANSACTION T1
		END CATCH
	END
	GO


IF OBJECT_ID('NewUser', 'U') IS NOT NULL 
BEGIN
	SET IDENTITY_INSERT dbo.[NewUser]  ON
END
GO	 

IF OBJECT_ID('NewUser', 'U') IS NOT NULL 
BEGIN
	BEGIN TRANSACTION T2
	BEGIN TRY
		INSERT INTO dbo.[NewUser](Id,UniqueId,Code,FirstName,SurName,UserName,StartDate,EndDate,RegisterDate,IsActive,IsAdmin,AdminLevel,[Password],PasswordBytes,AuthMode,IsMasterAdmin,[Image],EntityId,Email,TelNumber,[Type],RemainingCredit,AllowedStockCount)
			SELECT DISTINCT Id,-Id,Id,FirstName,SurName,UserName,StartDate,EndDate,RegisterDate,IsActive,ISNULL(IsAdmin, 0),AdminLevel,[Password],PasswordBytes,AuthMode,IsMasterAdmin,[Image],EntityId,Email,TelNumber,[Type],RemainingCredit,AllowedStockCount
			FROM #tempuser

		DROP TABLE dbo.[User]
		EXEC sp_rename 'NewUser', 'User'
		COMMIT TRANSACTION T2
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION T2
		DROP TABLE [NewUser]
		IF OBJECT_ID('dbo.DF_User_RegisterDate', 'D') IS NULL
		BEGIN
			ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_RegisterDate]  DEFAULT (getdate()) FOR [RegisterDate]
		END
	END CATCH
END
GO

IF COL_LENGTH('dbo.User', 'Code') IS NOT NULL
BEGIN
	SET IDENTITY_INSERT dbo.[User] OFF
	UPDATE [User] SET [UniqueId] = 123456789 WHERE [Code] = 123456789
END
