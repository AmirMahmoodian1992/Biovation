using System;
using System.Configuration;
using System.Reflection;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Migration;
using DbUp;

namespace Biovation.Brands.ZK
{
    public static class Migration
    {
        public static bool MigrateUp()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["CONNECTIONSTRING".ToUpper()].ConnectionString;

            try
            {
                var everyTimeUpgrader =
                        DeployChanges.To
                            .SqlDatabase(connectionString)
                            //.WithScriptsEmbeddedInAssembly(Assembly.UnsafeLoadFrom(HomeDirectory + @"Biovation.Brands.Zk.dll"))
                            //.WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), name => name.Contains("SP") || name.Contains("Functions") || name.Contains("Triggers"))
                            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                            .WithPreprocessor(new ScriptPreprocessor())
                            .JournalTo(new Journaling("ZKTeco"))
                            //.WithVariable(@"dbName", unisDatabaseName)
                            //.WithTransactionPerScript()
                            //.LogToSqlContext()
                            .LogToConsole()
                            .Build();

                string errMessage;
                everyTimeUpgrader.TryConnect(out errMessage);

                if (!string.IsNullOrEmpty(errMessage))
                {
                    Logger.Log(errMessage);
                    return false;
                }

                else
                {
                    var result = everyTimeUpgrader.PerformUpgrade();

                    if (!result.Successful)
                    {
                        Logger.Log(result.Error.ToString());
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Data.ToString());
                return false;
            }

            return true;
        }
    }
}
