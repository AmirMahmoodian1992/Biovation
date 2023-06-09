IF COL_LENGTH('dbo.LicensePlate', 'LicencePart1') IS Not NULL And COL_LENGTH('dbo.LicensePlate', 'FirstPart') IS  NULL
BEGIN
EXEC sp_RENAME 'LicensePlate.LicencePart1' , 'FirstPart', 'COLUMN'
EXEC sp_RENAME 'LicensePlate.LicencePart2' , 'SecondPart', 'COLUMN'
EXEC sp_RENAME 'LicensePlate.LicencePart3' , 'ThirdPart', 'COLUMN'
EXEC sp_RENAME 'LicensePlate.LicencePart4' , 'FourthPart', 'COLUMN'
END
GO

IF COL_LENGTH('dbo.LicensePlate', 'FirstPart') IS NULL
BEGIN
ALTER TABLE [dbo].[LicensePlate]
	ADD [FirstPart] [nvarchar](5) Null,
		[SecondPart] [nvarchar](5) Null,
		[ThirdPart] [nvarchar](5) Null,
		[FourthPart] [nvarchar](5) Null,
		[IsActive] [bit] NULL,
		[StartDate] [Date] NULL,
		[EndDate] [date] NULL,
		[StartTime] [time] NULL,
		[EndTime] [time] NULL
END
GO

UPDATE [dbo].[LicensePlate]
				SET [FirstPart] = SUBSTRING([dbo].[LicensePlate].[LicensePlate],1,2), [SecondPart] = SUBSTRING([dbo].[LicensePlate].[LicensePlate],3,1), [ThirdPart] = SUBSTRING([dbo].[LicensePlate].[LicensePlate],4,3), [FourthPart] = SUBSTRING([dbo].[LicensePlate].[LicensePlate],7,2)
				--WHERE LEN([dbo].[LicensePlate].[LicensePlate]) = LIKE '[۰-۹][۰-۹][آ-ی][۰-۹][۰-۹][۰-۹][۰-۹][۰-۹]'
				WHERE  [LicensePlate] LIKE N'[۰-۹][۰-۹][آ-ی][۰-۹][۰-۹][۰-۹][۰-۹][۰-۹]' AND FirstPart IS NULL 
GO
