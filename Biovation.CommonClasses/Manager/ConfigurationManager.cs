using Microsoft.Extensions.Configuration;
using System;
using System.Configuration;

namespace Biovation.CommonClasses.Manager
{
    public class BiovationConfigurationManager
    {
        public IConfiguration Configuration { get; set; }
        private string LoginKey { get; } = "BiovationLoginKey";
        //private string ServiceKey { get; } = "BiovationServiceKey"; 

        public BiovationConfigurationManager(IConfiguration configuration)
        {
            Configuration = configuration;
            //_settingService = settingService;
        }

        public string JwtIssuer()
        {
            return Configuration.GetSection("Jwt")["Issuer"];
        }

        public string JwtAudience()
        {
            return Configuration.GetSection("Jwt")["Audience"];
        }

        public string JwtLoginKey()
        {
            return LoginKey;
        }

        public string ConnectionStringProviderName()
        {
            return Configuration.GetSection("ConnectionStrings")["ProviderName"];
        }

        public string ConnectionStringInitialCatalog()
        {
            return Configuration["InitialCatalog"] ?? Configuration.GetSection("ConnectionStrings")["InitialCatalog"];
        }

        public string ConnectionStringDataSource()
        {
            return Configuration["DataSource"] ?? Configuration.GetSection("ConnectionStrings")["DataSource"];
        }

        public string ConnectionStringWorkstationId()
        {
            return Configuration.GetSection("ConnectionStrings")["WorkstationId"];
        }

        public string ConnectionStringParameters()
        {
            return Configuration.GetSection("ConnectionStrings")["Parameters"];
        }

        public string ConnectionStringUsername()
        {
            return Configuration["DBUsername"] ?? Configuration.GetSection("ConnectionStrings")["Username"];
        }

        public string ConnectionStringPassword()
        {
            return Configuration["Password"] ?? Configuration.GetSection("ConnectionStrings")["Password"];
        }

        public Uri LogMonitoringApiUrl
        {
            get
            {
                try
                {
                    try
                    {
                        var useHttps = Configuration.GetSection("AppSettings")["LegoServerAddress"]
                            ?.ToLowerInvariant().Contains(@"https://") == true;


                        var rawUrl = Configuration.GetSection("AppSettings")["LegoServerAddress"]?.ToLowerInvariant()
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

                            return new Uri(baseUri, string.IsNullOrWhiteSpace(relativeUri) ? "/Lego.Web" : relativeUri + "/api/Biovation");
                        }
                        else
                        {
                            var baseUri = (!rawUrl.Contains(":") ? new UriBuilder(useHttps ? Uri.UriSchemeHttps : Uri.UriSchemeHttp, rawUrl, useHttps ? 443 : 80)
                                : new UriBuilder(useHttps ? Uri.UriSchemeHttps : Uri.UriSchemeHttp, rawUrl.Split(':')[0],
                                    Convert.ToInt32(rawUrl.Split(':')[1]))).Uri;

                            return new Uri(baseUri, "/Lego.Web/api/Biovation");
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

        public Uri BiovationServerUri
        {
            get
            {
                try
                {
                    try
                    {
                        var useHttps = Configuration.GetSection("AppSettings")["BiovationServerUrl"]
                            ?.ToLowerInvariant().Contains(@"https://") == true;


                        var rawUrl = Configuration.GetSection("AppSettings")["BiovationServerUrl"]?.ToLowerInvariant()
                            .Replace(@"http://", "").Replace(@"https://", "");
                        if (rawUrl is null)
                            return default;

                        var slashIndex = rawUrl.IndexOf("/", StringComparison.InvariantCultureIgnoreCase);
                        if (slashIndex > 0)
                        {
                            var relativeUri = rawUrl.Substring(slashIndex).Trim('/');
                            rawUrl = rawUrl.Substring(0, slashIndex).Trim('/');
                            var baseUri = (!rawUrl.Contains(":") ? new UriBuilder(useHttps ? Uri.UriSchemeHttps : Uri.UriSchemeHttp, rawUrl, useHttps ? 9039 : 9038)
                                : new UriBuilder(useHttps ? Uri.UriSchemeHttps : Uri.UriSchemeHttp, rawUrl.Split(':')[0],
                                    Convert.ToInt32(rawUrl.Split(':')[1].Split('/')[0]))).Uri;

                            return new Uri(baseUri, relativeUri + "/Biovation/api");
                        }
                        else
                        {
                            var baseUri = (!rawUrl.Contains(":") ? new UriBuilder(useHttps ? Uri.UriSchemeHttps : Uri.UriSchemeHttp, rawUrl, useHttps ? 9039 : 9038)
                                : new UriBuilder(useHttps ? Uri.UriSchemeHttps : Uri.UriSchemeHttp, rawUrl.Split(':')[0],
                                    Convert.ToInt32(rawUrl.Split(':')[1]))).Uri;

                            return new Uri(baseUri, "/Biovation/api");
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

        public string DefaultToken { get; set; } = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VyIjoie1wiSWRcIjoxMjM0NTY3ODksXCJDb2RlXCI6MTIzNDU2Nzg5LFwiVW5pcXVlSWRcIjoxMjM0NTY3ODksXCJVc2VyTmFtZVwiOlwiQWRtaW5cIixcIlJlZ2lzdGVyRGF0ZVwiOlwiMjAyMC0xMi0wMVQwMDoxNjowMFwiLFwiRGVwYXJ0bWVudE5hbWVcIjpudWxsLFwiVGVsTnVtYmVyXCI6bnVsbCxcIkltYWdlQnl0ZXNcIjpudWxsLFwiSW1hZ2VcIjpudWxsLFwiRmlyc3ROYW1lXCI6XCJBZG1pblwiLFwiU3VyTmFtZVwiOlwiQWRtaW5pc3RyYXRvclwiLFwiRnVsbE5hbWVcIjpcIjEyMzQ1Njc4OV9BZG1pbiBBZG1pbmlzdHJhdG9yXCIsXCJQYXNzd29yZFwiOm51bGwsXCJQYXNzd29yZEJ5dGVzXCI6bnVsbCxcIlN0YXJ0RGF0ZVwiOlwiMjAyMC0xMi0wMVQwMDoxNjowMFwiLFwiRW5kRGF0ZVwiOlwiMjAyMC0xMi0wMVQwMDoxNjowMFwiLFwiQWRtaW5MZXZlbFwiOjAsXCJBdXRoTW9kZVwiOjAsXCJFbWFpbFwiOm51bGwsXCJUeXBlXCI6MSxcIkVudGl0eUlkXCI6MSxcIklzQWN0aXZlXCI6dHJ1ZSxcIklzQWRtaW5cIjpmYWxzZSxcIlJlbWFpbmluZ0NyZWRpdFwiOjAuMCxcIkFsbG93ZWRTdG9ja0NvdW50XCI6MCxcIkZpbmdlclRlbXBsYXRlc1wiOltdLFwiRmFjZVRlbXBsYXRlc1wiOltdLFwiSWRlbnRpdHlDYXJkXCI6bnVsbH0iLCJqdGkiOiJhMzE3MzEwMC1hN2NiLTQyMDctOWViOS1mY2Y1OWJlYmIzNGMiLCJleHAiOjE2MzgzMDU3MDB9.07kH1oI5EYxAnu-iBIZ-CX8ycAaNPo7NwUjURBZKehU";
        public string KasraAdminToken { get; set; } = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VyIjoie1wiSWRcIjoxMjM0NTY3ODksXCJDb2RlXCI6MTIzNDU2Nzg5LFwiVW5pcXVlSWRcIjoxMjM0NTY3ODksXCJVc2VyTmFtZVwiOlwiQWRtaW5cIixcIlJlZ2lzdGVyRGF0ZVwiOlwiMjAyMC0xMi0wMVQwMDoxNjowMFwiLFwiRGVwYXJ0bWVudE5hbWVcIjpudWxsLFwiVGVsTnVtYmVyXCI6bnVsbCxcIkltYWdlQnl0ZXNcIjpudWxsLFwiSW1hZ2VcIjpudWxsLFwiRmlyc3ROYW1lXCI6XCJBZG1pblwiLFwiU3VyTmFtZVwiOlwiQWRtaW5pc3RyYXRvclwiLFwiRnVsbE5hbWVcIjpcIjEyMzQ1Njc4OV9BZG1pbiBBZG1pbmlzdHJhdG9yXCIsXCJQYXNzd29yZFwiOm51bGwsXCJQYXNzd29yZEJ5dGVzXCI6bnVsbCxcIlN0YXJ0RGF0ZVwiOlwiMjAyMC0xMi0wMVQwMDoxNjowMFwiLFwiRW5kRGF0ZVwiOlwiMjAyMC0xMi0wMVQwMDoxNjowMFwiLFwiQWRtaW5MZXZlbFwiOjAsXCJBdXRoTW9kZVwiOjAsXCJFbWFpbFwiOm51bGwsXCJUeXBlXCI6MSxcIkVudGl0eUlkXCI6MSxcIklzQWN0aXZlXCI6dHJ1ZSxcIklzQWRtaW5cIjpmYWxzZSxcIlJlbWFpbmluZ0NyZWRpdFwiOjAuMCxcIkFsbG93ZWRTdG9ja0NvdW50XCI6MCxcIkZpbmdlclRlbXBsYXRlc1wiOltdLFwiRmFjZVRlbXBsYXRlc1wiOltdLFwiSWRlbnRpdHlDYXJkXCI6bnVsbH0iLCJqdGkiOiJhMzE3MzEwMC1hN2NiLTQyMDctOWViOS1mY2Y1OWJlYmIzNGMiLCJleHAiOjE2MzgzMDU3MDB9.07kH1oI5EYxAnu-iBIZ-CX8ycAaNPo7NwUjURBZKehU";
        

        public bool UseHealthCheck
        {
            get
            {
                try
                {
                    return string.Equals(Configuration.GetSection("AppSettings")["UseHealthCheck"] ?? bool.TrueString, bool.TrueString, StringComparison.InvariantCultureIgnoreCase);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return false;
                }
            }

            set => Configuration.GetSection("AppSettings")["UseHealthCheck"] = value.ToString();
        }

        public bool BroadcastToMessageBus
        {
            get
            {
                try
                {
                    return string.Equals(Configuration.GetSection("AppSettings")["BroadcastToMessageBus"] ?? bool.FalseString, bool.TrueString, StringComparison.InvariantCultureIgnoreCase);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return false;
                }
            }

            set => Configuration.GetSection("AppSettings")["BroadcastToMessageBus"] = value.ToString();
        }

        public bool BroadcastToApi
        {
            get
            {
                try
                {
                    return string.Equals(Configuration.GetSection("AppSettings")["BroadcastToApi"] ?? bool.TrueString, bool.TrueString, StringComparison.InvariantCultureIgnoreCase);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return false;
                }
            }

            set => Configuration.GetSection("AppSettings")["BroadcastToApi"] = value.ToString();
        }

        public bool MigrateUp
        {
            get
            {
                try
                {
                    return string.Equals(Configuration.GetSection("AppSettings")["MigrateUp"] ?? bool.TrueString, bool.TrueString, StringComparison.InvariantCultureIgnoreCase);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return false;
                }
            }

            set => Configuration.GetSection("AppSettings")["MigrateUp"] = value.ToString();
        }

        public int SupremaDevicesConnectionPort
        {
            get
            {
                try
                {
                    return Convert.ToInt32(Configuration.GetSection("AppSettings")["SupremaDevicesConnectionPort"] ?? "1480");
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return default;
                }
            }
        }

        public int VirdiDevicesConnectionPort
        {
            get
            {
                try
                {
                    return Convert.ToInt32(ConfigurationManager.AppSettings["VirdiDevicesConnectionPort"] ?? "9870");
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return default;
                }
            }
        }


        public bool GetAllLogWhenConnect
        {
            get
            {
                try
                {
                    return string.Equals(Configuration.GetSection("AppSettings")["GetAllLogWhenConnect"] ?? bool.TrueString, bool.TrueString, StringComparison.InvariantCultureIgnoreCase);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return false;
                }
            }
        }

        public bool ClearLogAfterRetrieving
        {
            get
            {
                try
                {
                    return string.Equals(Configuration.GetSection("AppSettings")["ClearLogAfterRetrieving"] ?? bool.FalseString, bool.TrueString, StringComparison.InvariantCultureIgnoreCase);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return false;
                }
            }
        }

        public bool ShowLiveImageInMonitoring
        {
            get
            {
                try
                {
                    return string.Equals(Configuration.GetSection("AppSettings")["ShowLiveImageInMonitoring"], bool.TrueString, StringComparison.InvariantCultureIgnoreCase);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return default;
                }
            }
        }

        public string SoftwareLockAddress
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["SoftwareLockAddress"]?.ToLowerInvariant();
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return default;
                }
            }
        }

        public int SoftwareLockPort
        {
            get
            {
                try
                {
                    return Convert.ToInt32(ConfigurationManager.AppSettings["SoftwareLockPort"]);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return default;
                }
            }
        }

        public string KafkaServerAddress
        {
            get
            {
                try
                {
                    try
                    {
                        return Configuration.GetSection("AppSettings")["KafkaServerAddress"]?.ToLowerInvariant()
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

        public bool LockDevice
        {
            get
            {
                try
                {
                    return string.Equals(Configuration.GetSection("AppSettings")["LockDevice"], bool.FalseString, StringComparison.InvariantCultureIgnoreCase);
                }
                catch (Exception exception)
                {
                    Logger.Log(exception);
                    return false;
                }
            }
        }

        //public static int MaxaDevicesConnectionPort
        //{
        //    get
        //    {
        //        try
        //        {
        //            return Convert.ToInt32(ConfigurationManager.AppSettings["MaxaDevicesConnectionPort"]);
        //        }
        //        catch (Exception exception)
        //        {
        //            Logger.Log(exception);
        //            return default;
        //        }
        //    }
        //}
    }
}
