
UPDATE [dbo].[AuthModeMap] SET [BrandId] = 15000 + [BrandId] WHERE CAST([BrandId] AS int) < 100

IF NOT EXISTS (SELECT * FROM [dbo].[AuthModeMap] WHERE [BrandId] = 15001 AND [AuthMode] = 0 AND [BioCode] = 1)
	INSERT INTO [dbo].[AuthModeMap] ([BioCode], [BrandId], [AuthMode], [BioTitle]) VALUES (1, 15001, 0, N'FingerTemplate')
ELSE IF EXISTS (SELECT * FROM [dbo].[AuthModeMap] WHERE [BrandId] = 15001 AND [AuthMode] = 0 AND [BioCode] <> 1)
	BEGIN
		--UPDATE [dbo].[AuthModeMap] SET [BioCode] = 1, [BioTitle] = N'FingerTemplate' WHERE [BrandId] = 15001 AND [AuthMode] = 3
		DELETE FROM [dbo].[AuthModeMap] WHERE [BrandId] = 15001 AND [AuthMode] = 0 AND [BioCode] <> 1
		INSERT INTO [dbo].[AuthModeMap] ([BioCode], [BrandId], [AuthMode], [BioTitle]) VALUES (1, 15001, 0, N'FingerTemplate')
	END

IF NOT EXISTS (SELECT * FROM [dbo].[AuthModeMap] WHERE [BrandId] = 15002 AND [AuthMode] = 58)
	INSERT INTO [dbo].[AuthModeMap] ([BioCode], [BrandId], [AuthMode], [BioTitle]) VALUES (1, 15002, 58, N'FingerTemplate')

IF NOT EXISTS (SELECT * FROM [dbo].[AuthModeMap] WHERE [BrandId] = 15004 AND [AuthMode] = 1)
	INSERT INTO [dbo].[AuthModeMap] ([BioCode], [BrandId], [AuthMode], [BioTitle]) VALUES (1, 15004, 1, N'FingerTemplate')

--IF NOT EXISTS (SELECT * FROM [dbo].[AuthModeMap] WHERE [BrandId] = 15001 AND [AuthMode] = 3 AND [BioCode] = 2)
--	INSERT INTO [dbo].[AuthModeMap] ([BioCode], [BrandId], [AuthMode], [BioTitle]) VALUES (2, 15001, 3, N'Card')
--ELSE IF EXISTS (SELECT * FROM [dbo].[AuthModeMap] WHERE [BrandId] = 15001 AND [AuthMode] = 3 AND [BioCode] <> 2)
--	BEGIN
--		--UPDATE [dbo].[AuthModeMap] SET [BioCode] = 2, [BioTitle] = N'Card' WHERE [BrandId] = 15001 AND [AuthMode] = 3
--		DELETE FROM [dbo].[AuthModeMap] WHERE [BrandId] = 15001 AND [AuthMode] = 3 AND [BioCode] <> 2
--		INSERT INTO [dbo].[AuthModeMap] ([BioCode], [BrandId], [AuthMode], [BioTitle]) VALUES (2, 15001, 3, N'Card')
--	END

--IF NOT EXISTS (SELECT * FROM [dbo].[AuthModeMap] WHERE [BrandId] = 15004 AND [AuthMode] = 4)
--	INSERT INTO [dbo].[AuthModeMap] ([BioCode], [BrandId], [AuthMode], [BioTitle]) VALUES (2, 15004, 4, N'Card')

--IF NOT EXISTS (SELECT * FROM [dbo].[AuthModeMap] WHERE [BrandId] = 15001 AND [AuthMode] = 5)
--	INSERT INTO [dbo].[AuthModeMap] ([BioCode], [BrandId], [AuthMode], [BioTitle]) VALUES (3, 15001, 5, N'FaceTemplate')

--IF NOT EXISTS (SELECT * FROM [dbo].[AuthModeMap] WHERE [BrandId] = 15004 AND [AuthMode] = 15)
--	INSERT INTO [dbo].[AuthModeMap] ([BioCode], [BrandId], [AuthMode], [BioTitle]) VALUES (3, 15004, 15, N'FaceTemplate')