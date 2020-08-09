using Biovation.CommonClasses.Service;
using System;
using System.Configuration;

namespace Biovation.CommonClasses.Manager
{
    public static class ConfigurationManager
    {
        private static readonly SettingService SettingService = new SettingService();

        public static Uri LogMonitoringApiUrl
        {
            get
            {
                try
                {
                    try
                    {
                        var useHttps = System.Configuration.ConfigurationManager.AppSettings["LegoServerAddress"]
                            ?.ToLowerInvariant().Contains(@"https://") == true;

                        var rawUrl = System.Configuration.ConfigurationManager.AppSettings["LegoServerAddress"]?.ToLowerInvariant()
                            .Replace(@"http://", "").Replace(@"https://", "");
                        if (rawUrl is null)
                            return default;

                        var slashIndex = rawUrl.IndexOf("/", StringComparison.InvariantCultureIgnoreCase);
                        if (slashIndex > 0)
                        {
                            var relativeUri = rawUrl.Substring(slashIndex).Trim('/');
                            rawUrl = rawUrl.Substring(0, slashIndex).Trim('/');
                            var baseUri = (!rawUrl.Contains(":") ? new UriBuilder(useHttps ? Uri.UriSchemeHttps : Uri.UriSchemeHttp, rawUrl, useHttps ? 443 : 80)
                                : new UriBuilder(useHttps ? Uri.UriSchemeHttps : Uri.UriSchemeHttp, rawUrl.Split(':')[0],
                                    Convert.ToInt32(rawUrl.Split(':')[1].Split('/')[0]))).Uri;

                            return new Uri(baseUri, relativeUri + "/api/Biovation");
                        }
                        else
                        {
                            var baseUri = (!rawUrl.Contains(":") ? new UriBuilder(useHttps ? Uri.UriSchemeHttps : Uri.UriSchemeHttp, rawUrl, useHttps ? 443 : 80)
                                : new UriBuilder(useHttps ? Uri.UriSchemeHttps : Uri.UriSchemeHttp, rawUrl.Split(':')[0],
                                    Convert.ToInt32(rawUrl.Split(':')[1]))).Uri;

                            return new Uri(baseUri, "/api/Biovation");
                        }
                    }
                    catch (Exception)
                    {
                        return default;
                    }
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return default;
                }
            }
        }

        public static int BiovationWebServerPort
        {
            get
            {
                try
                {
                    return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["BiovationEventWebServerPort"] ?? "9038");
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return default;
                }
            }
        }

        public static int BiovationSocketServerPort
        {
            get
            {
                try
                {
                    return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["BiovationEventServerSocketPort"] ?? "6000");
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return default;
                }
            }
        }

        public static bool MigrateUp
        {
            get
            {
                try
                {
                    return string.Equals(System.Configuration.ConfigurationManager.AppSettings["MigrateUp"] ?? bool.TrueString, bool.TrueString, StringComparison.InvariantCultureIgnoreCase);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return false;
                }
            }

            set
            {
                var config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                //config.AppSettings.Settings["MigrateUp"].Value = bool.FalseString;
                config.AppSettings.Settings["MigrateUp"].Value = value.ToString();
                config.Save(ConfigurationSaveMode.Modified, true);
                System.Configuration.ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.SectionName);
            }
        }

        public static bool WriteLogToDatabase
        {
            get
            {
                try
                {
                    return string.Equals(System.Configuration.ConfigurationManager.AppSettings["WriteLogToDatabase"] ?? bool.TrueString, bool.TrueString, StringComparison.InvariantCultureIgnoreCase);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return false;
                }
            }
        }

        public static bool WriteLogToFile
        {
            get
            {
                try
                {
                    return string.Equals(System.Configuration.ConfigurationManager.AppSettings["WriteLogToFile"] ?? bool.FalseString, bool.TrueString, StringComparison.InvariantCultureIgnoreCase);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return false;
                }
            }
        }

        public static string LogConnectionString
        {
            get
            {
                try
                {
                    return System.Configuration.ConfigurationManager.ConnectionStrings["LogConnectionString".ToUpper()]?.ConnectionString ?? System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString".ToUpper()].ConnectionString;
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString".ToUpper()].ConnectionString;
                }
            }
        }

        public static int SupremaDevicesConnectionPort
        {
            get
            {
                try
                {
                    return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SupremaDevicesConnectionPort"] ?? "1480");
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return default;
                }
            }
        }

        public static int VirdiDevicesConnectionPort
        {
            get
            {
                try
                {
                    return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["VirdiDevicesConnectionPort"] ?? "9870");
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return default;
                }
            }
        }

        public static int MaxaDevicesConnectionPort
        {
            get
            {
                try
                {
                    return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxaDevicesConnectionPort"]);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return default;
                }
            }
        }

        public static bool GetAllLogWhenConnect
        {
            get
            {
                try
                {
                    return string.Equals(System.Configuration.ConfigurationManager.AppSettings["GetAllLogWhenConnect"] ?? bool.TrueString, bool.TrueString, StringComparison.InvariantCultureIgnoreCase);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return false;
                }
            }
        }

        public static bool ClearLogAfterRetrieving
        {
            get
            {
                try
                {
                    return string.Equals(System.Configuration.ConfigurationManager.AppSettings["ClearLogAfterRetrieving"] ?? bool.FalseString, bool.TrueString, StringComparison.InvariantCultureIgnoreCase);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return false;
                }
            }
        }

        public static string MinimumFileLogLevel
        {
            get
            {
                try
                {
                    return System.Configuration.ConfigurationManager.AppSettings["MinimumFileLogLevel"]?.ToLowerInvariant();
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return default;
                }
            }
        }

        public static string MinimumConsoleLogLevel
        {
            get
            {
                try
                {
                    return System.Configuration.ConfigurationManager.AppSettings["MinimumConsoleLogLevel"]?.ToLowerInvariant();
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return default;
                }
            }
        }

        public static string MinimumDatabaseLogLevel
        {
            get
            {
                try
                {
                    return System.Configuration.ConfigurationManager.AppSettings["MinimumDatabaseLogLevel"]?.ToLowerInvariant();
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return default;
                }
            }
        }

        public static bool ShowLiveImageInMonitoring
        {
            get
            {
                try
                {
                    var databaseValue = SettingService.GetSetting("ShowLiveImageInMonitoring").Result;
                    return string.Equals(databaseValue ?? (System.Configuration.ConfigurationManager.AppSettings["ShowLiveImageInMonitoring"] ?? bool.FalseString), bool.TrueString, StringComparison.InvariantCultureIgnoreCase);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return default;
                }
            }
        }

        public static string SoftwareLockAddress
        {
            get
            {
                try
                {
                    return System.Configuration.ConfigurationManager.AppSettings["SoftwareLockAddress"]?.ToLowerInvariant();
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return default;
                }
            }
        }

        public static int SoftwareLockPort
        {
            get
            {
                try
                {
                    return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SoftwareLockPort"]);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return default;
                }
            }
        }

        public static string KafkaServerAddress
        {
            get
            {
                try
                {
                    try
                    {
                        return System.Configuration.ConfigurationManager.AppSettings["KafkaServerAddress"]?.ToLowerInvariant()
                            .Replace(@"http://", "").Replace(@"https://", "");

                    }
                    catch (Exception)
                    {
                        return default;
                    }
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return default;
                }
            }
        }

        // For EOS module to connect directly to kasra database and add records to att.attendance
        public static bool OldRepository
        {
            get
            {
                try
                {
                    return string.Equals(System.Configuration.ConfigurationManager.AppSettings["OldRepository"], bool.TrueString, StringComparison.InvariantCultureIgnoreCase);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return false;
                }
            }
        }
    }
}
