CREATE TABLE [Rly].Entrance
	(
	Id int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	Code BIGint NOT NULL,
	Name nvarchar(100) NULL,
	Description nvarchar(MAX) NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO