SELECT [Id], REPLACE([ScriptName], 'Biovation.Server.SQL_Scripts', 'Biovation.Data.Queries.Scripts') AS [ScriptName], [Applied], [LastUpdate] INTO #TEMP FROM [dbo].[_MigrationHistory]
WHERE [ScriptName] LIKE 'Biovation.Server.SQL_Scripts%'

INSERT INTO [dbo].[_MigrationHistory]
           ([ScriptName]
           ,[Applied]
           ,[LastUpdate])
SELECT [ScriptName], [Applied], [LastUpdate] FROM #TEMP
WHERE [ScriptName] NOT IN (SELECT [SM].[ScriptName] FROM [dbo].[_MigrationHistory] AS [PM] JOIN #TEMP AS [SM]
ON [PM].[ScriptName] = [SM].[ScriptName])
