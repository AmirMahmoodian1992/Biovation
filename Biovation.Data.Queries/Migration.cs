using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Migration;
using DataAccessLayerCore.Domain;
using DbUp;

namespace Biovation.Data.Queries
{
    public static class Migration
    {
        public static bool MigrateUp(DatabaseConnectionInfo databaseConnectionInfo)
        {
            //var connectionString = ConfigurationManager.ConnectionStrings["CONNECTIONSTRING".ToUpper()].ConnectionString;

            try
            {
                var everyTimeUpgrader =
                    DeployChanges.To
                        .SqlDatabase(databaseConnectionInfo.GetConnectionString())
                        .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                        .WithExecutionTimeout(TimeSpan.FromSeconds(180))
                        .WithPreprocessor(new ScriptPreprocessor())
                        .JournalTo(new Journaling("Main", databaseConnectionInfo))
                        //.WithTransactionPerScript()
                        .LogToAutodetectedLog()
                        .LogToConsole()
                        .Build();

                everyTimeUpgrader.TryConnect(out var errMessage);

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
