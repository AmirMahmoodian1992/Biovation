IF COL_LENGTH('dbo.User', 'UniqueId') IS NULL And COL_LENGTH('dbo.User', 'Code') IS  NULL
	BEGIN 
		BEGIN TRANSACTION
		SELECT * INTO #tempuser
		FROM dbo.[User]

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


		DROP TABLE dbo.[User]


		CREATE TABLE [dbo].[User](
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

		COMMIT
	END
	GO

	SET IDENTITY_INSERT dbo.[User]  ON
	GO	 

	IF COL_LENGTH('dbo.User', 'UniqueId') IS NULL And COL_LENGTH('dbo.User', 'Code') IS  NULL
	BEGIN
		BEGIN TRANSACTION
		INSERT INTO dbo.[User](Id,UniqueId,Code,FirstName,SurName,UserName,StartDate,EndDate,RegisterDate,IsActive,IsAdmin,AdminLevel,[Password],PasswordBytes,AuthMode,IsMasterAdmin,[Image],EntityId,Email,TelNumber,[Type],RemainingCredit,AllowedStockCount)
			Select Id,-Id,Id,FirstName,SurName,UserName,StartDate,EndDate,RegisterDate,IsActive,IsAdmin,AdminLevel,[Password],PasswordBytes,AuthMode,IsMasterAdmin,[Image],EntityId,Email,TelNumber,[Type],RemainingCredit,AllowedStockCount
			from #tempuser
		COMMIT
	END
	GO

		SET IDENTITY_INSERT dbo.[User]  OFF

	GO
