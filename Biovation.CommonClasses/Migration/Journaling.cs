using DataAccessLayerCore;
using DataAccessLayerCore.Domain;
using DbUp.Engine;
using DbUp.SqlServer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Biovation.CommonClasses.Migration
{
    public sealed class Journaling : IJournal
    {
        private string Schema { get; }
        private string TableName { get; }
        private string ModuleName { get; }
        private DatabaseConnectionInfo ConnectionInfo { get; }

        /// <summary>
        /// برای ایجاد کانکشن به دیتابیس
        /// </summary>
        private static IConnectionFactory _connectionFactory;

        private static SqlServerObjectParser _sqlServerObjectParser;

        public Journaling(string moduleName, DatabaseConnectionInfo connectionInfo)
        {
            Schema = "dbo";
            TableName = "_MigrationHistory";
            ModuleName = moduleName;
            ConnectionInfo = connectionInfo;
            _connectionFactory = ConnectionHelper.GetConnection(ConnectionInfo);
            _sqlServerObjectParser = new SqlServerObjectParser();
        }

        private static bool VerifyTableExistsCommand(string tableName, string schemaName)
        {
            using var context = new DbContext(_connectionFactory);
            using var command = context.CreateCommand();
            command.CommandText =
                $"SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}' AND TABLE_SCHEMA = '{schemaName}'";
            var res = command.ExecuteScalar() as int?;
            return res == 1;
        }

        private static string CreateTableName(string schema, string table)
        {
            return string.IsNullOrEmpty(schema) ? _sqlServerObjectParser.QuoteIdentifier(table) : _sqlServerObjectParser.QuoteIdentifier(schema) + "." + _sqlServerObjectParser.QuoteIdentifier(table);
        }

        private static string CreatePrimaryKeyName(string table)
        {
            return _sqlServerObjectParser.QuoteIdentifier("PK_" + table + "_Id");
        }

        private static string CreateTableSql(string schema, string table)
        {
            var tableName = CreateTableName(schema, table);
            var primaryKeyConstraintName = CreatePrimaryKeyName(table);
            return $"CREATE TABLE {tableName} (\n\t[Id] int IDENTITY(1,1) NOT NULL CONSTRAINT {primaryKeyConstraintName} PRIMARY KEY,\n\t[ScriptName] nvarchar(255) NOT NULL,\n\t[Applied] datetime NOT NULL,\n\t[LastUpdate] datetime\n\n)";
        }

        private static string GetExecutedScriptsSql(string schema, string table)
        {
            return $"SELECT [ScriptName] FROM {CreateTableName(schema, table)} WHERE [ScriptName] NOT LIKE '%03.SP%' AND [ScriptName] NOT LIKE '%02.Functions%' AND [ScriptName] NOT LIKE '%04.Triggers%' AND [ScriptName] NOT LIKE '%05.Data%' AND [ScriptName] NOT LIKE '%06.View%' ORDER BY [ScriptName]";
        }

        public string[] GetExecutedScripts()
        {
            Logger.Log($"[{ModuleName}] : Fetching list of already executed scripts.");
            string[] result;
            if (!VerifyTableExistsCommand(TableName, Schema))
            {
                Logger.Log($"[{ModuleName}] : The {CreateTableName(Schema, TableName)} table could not be found. The database is assumed to be at version 0.");
                result = new string[0];
            }
            else
            {
                var scripts = new List<string>();

                using (var context = new DbContext(_connectionFactory))
                {
                    using var command = context.CreateCommand();
                    command.CommandText = GetExecutedScriptsSql(Schema, TableName);
                    command.CommandType = CommandType.Text;
                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        scripts.Add((string)reader[0]);
                    }
                }

                var entryAssemblyName = Assembly.GetEntryAssembly()?.GetName().Name;
                var newScriptNames = scripts.Select(sc => sc.Replace("Biovation.Server.SQL_Scripts", $"{entryAssemblyName ?? "Biovation.Data.Queries"}.Scripts")).ToList();
                scripts.AddRange(newScriptNames);
                result = scripts.ToArray();
            }

            return result;
        }

        public void StoreExecutedScript(SqlScript script, Func<IDbCommand> dbCommandFactory)
        {
            if (script.Name.Contains("02.Functions") || script.Name.Contains("03.SP") || script.Name.Contains("04.Triggers") || script.Name.Contains("05.Data") || script.Name.Contains("06.Data"))
            {
                using var context = new DbContext(_connectionFactory);
                using var command = context.CreateCommand();
                command.CommandText =
                    $@"IF NOT EXISTS (SELECT * FROM {CreateTableName(Schema, TableName)} WHERE [ScriptName] ='{script.Name}') 
                                BEGIN
                                INSERT INTO {CreateTableName(Schema, TableName)} (ScriptName, Applied) VALUES ('{script.Name}', '{DateTime.Now.ToString("G", CultureInfo.InvariantCulture)}')
                                END

                                ELSE
                                BEGIN
                                    UPDATE {CreateTableName(Schema, TableName)} SET LastUpdate = '{DateTime.Now.ToString("G", CultureInfo.InvariantCulture)}' WHERE [ScriptName] = '{script.Name}'
                                END";

                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }

            else
            {
                using var context = new DbContext(_connectionFactory);
                using var command = context.CreateCommand();
                command.CommandText =
                    $"INSERT INTO {CreateTableName(Schema, TableName)} (ScriptName, Applied) values ('{script.Name}', '{DateTime.Now.ToString("G", CultureInfo.InvariantCulture)}')";
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
        }

        public void EnsureTableExistsAndIsLatestVersion(Func<IDbCommand> dbCommandFactory)
        {
            if (VerifyTableExistsCommand(TableName, Schema)) return;
            Logger.Log($"[{ModuleName}] : Creating the {CreateTableName(Schema, TableName)} table");

            using var context = new DbContext(_connectionFactory);
            using var command = context.CreateCommand();
            command.CommandText = CreateTableSql(Schema, TableName);
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();

            Logger.Log($"[{ModuleName}] : The {CreateTableName(Schema, TableName)} table has been created");
        }
    }
}
