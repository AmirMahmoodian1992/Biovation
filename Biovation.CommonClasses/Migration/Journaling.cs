using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using DataAccessLayerCore;
using DataAccessLayerCore.Domain;
using DbUp.Engine;
using DbUp.Support.SqlServer;

namespace Biovation.CommonClasses.Migration
{
    public sealed class Journaling : IJournal
    {
        private string Schema { get; set; }
        private string TableName { get; set; }
        private string ModuleName { get; set; }
        private DatabaseConnectionInfo ConnectionInfo { get; set; }

        /// <summary>
        /// برای ایجاد کانکشن به دیتابیس
        /// </summary>
        private static IConnectionFactory _connectionFactory;

        public Journaling(string moduleName, DatabaseConnectionInfo connectionInfo)
        {
            Schema = "dbo";
            TableName = "_MigrationHistory";
            ModuleName = moduleName;
            ConnectionInfo = connectionInfo;
            _connectionFactory = ConnectionHelper.GetConnection(ConnectionInfo);
        }

        public Journaling(string schema, string tableName, string moduleName, DatabaseConnectionInfo connectionInfo)
        {
            Schema = schema;
            TableName = tableName;
            ModuleName = moduleName;
            ConnectionInfo = connectionInfo;
            _connectionFactory = ConnectionHelper.GetConnection(ConnectionInfo);
        }

        private static bool VerifyTableExistsCommand(string tableName, string schemaName)
        {
            using (var context = new DbContext(_connectionFactory))
            {
                using (var command = context.CreateCommand())
                {
                    command.CommandText =
                $"SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}' AND TABLE_SCHEMA = '{schemaName}'";
                    var res = command.ExecuteScalar() as int?;
                    return res == 1;
                }
            }
        }

        private static string CreateTableName(string schema, string table)
        {
            return string.IsNullOrEmpty(schema) ? SqlObjectParser.QuoteSqlObjectName(table) : SqlObjectParser.QuoteSqlObjectName(schema) + "." + SqlObjectParser.QuoteSqlObjectName(table);
        }

        private static string CreatePrimaryKeyName(string table)
        {
            return SqlObjectParser.QuoteSqlObjectName("PK_" + table + "_Id");
        }

        private static string CreateTableSql(string schema, string table)
        {
            var tableName = CreateTableName(schema, table);
            var primaryKeyConstraintName = CreatePrimaryKeyName(table);
            return $"CREATE TABLE {tableName} (\n\t[Id] int IDENTITY(1,1) NOT NULL CONSTRAINT {primaryKeyConstraintName} PRIMARY KEY,\n\t[ScriptName] nvarchar(255) NOT NULL,\n\t[Applied] datetime NOT NULL,\n\t[LastUpdate] datetime\n\n)";
        }

        private static string GetExecutedScriptsSql(string schema, string table)
        {
            return $"SELECT [ScriptName] FROM {CreateTableName(schema, table)} WHERE [ScriptName] NOT LIKE '%SP%' AND [ScriptName] NOT LIKE '%Functions%' AND [ScriptName] NOT LIKE '%Triggers%' AND [ScriptName] NOT LIKE '%Data%' ORDER BY [ScriptName]";
        }

        public string[] GetExecutedScripts()
        {
            Logger.Log($"[{ModuleName}] : Fetching list of already executed scripts.");
            string[] result;
            if (!VerifyTableExistsCommand(TableName, Schema))
            {
                Logger.Log($"[{ModuleName}] : The {0} table could not be found. The database is assumed to be at version 0.", CreateTableName(Schema, TableName));
                result = new string[0];
            }
            else
            {
                var scripts = new List<string>();

                using (var context = new DbContext(_connectionFactory))
                {
                    using (var command = context.CreateCommand())
                    {
                        command.CommandText = GetExecutedScriptsSql(Schema, TableName);
                        command.CommandType = CommandType.Text;
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                scripts.Add((string)reader[0]);
                            }
                        }
                    }
                }

                result = scripts.ToArray();
            }
            return result;
        }

        public void StoreExecutedScript(SqlScript script)
        {
            if (!VerifyTableExistsCommand(TableName, Schema))
            {
                Logger.Log($"[{ModuleName}] : Creating the {0} table", CreateTableName(Schema, TableName));

                using (var context = new DbContext(_connectionFactory))
                {
                    using (var command = context.CreateCommand())
                    {
                        command.CommandText = CreateTableSql(Schema, TableName);
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();

                        Logger.Log($"[{ModuleName}] : The {0} table has been created", CreateTableName(Schema, TableName));
                    }
                }
            }

            if (script.Name.Contains("SP") || script.Name.Contains("Functions") || script.Name.Contains("Triggers") || script.Name.Contains("Data"))
            {
                using (var context = new DbContext(_connectionFactory))
                {
                    using (var command = context.CreateCommand())
                    {
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
                }
            }

            else
            {
                using (var context = new DbContext(_connectionFactory))
                {
                    using (var command = context.CreateCommand())
                    {
                        command.CommandText =
                            $"INSERT INTO {CreateTableName(Schema, TableName)} (ScriptName, Applied) values ('{script.Name}', '{DateTime.Now.ToString("G", CultureInfo.InvariantCulture)}')";
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
