


CREATE TABLE [Rly].Entrance
	(
	Id int IDENTITY(1,1) PRIMARY KEY,
	Name nvarchar(100) NULL,
	Description nvarchar(MAX) NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO

