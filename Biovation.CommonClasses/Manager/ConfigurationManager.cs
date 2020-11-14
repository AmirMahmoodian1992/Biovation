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

        public string DefaultToken { get; set; } = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VyIjoie1wiSWRcIjoxLFwiQ29kZVwiOjEyMzQ1Njc4OSxcIlVuaXF1ZUlkXCI6MTIzNDU2Nzg5LFwiVXNlck5hbWVcIjpcIkFkbWluXCIsXCJSZWdpc3RlckRhdGVcIjpcIjIwMjAtMTAtMjFUMTY6Mjk6MDBcIixcIkRlcGFydG1lbnROYW1lXCI6bnVsbCxcIlRlbE51bWJlclwiOm51bGwsXCJJbWFnZUJ5dGVzXCI6bnVsbCxcIkltYWdlXCI6bnVsbCxcIkZpcnN0TmFtZVwiOlwiQWRtaW5cIixcIlN1ck5hbWVcIjpcIkFkbWluaXN0cmF0b3JcIixcIkZ1bGxOYW1lXCI6XCIxX0FkbWluIEFkbWluaXN0cmF0b3JcIixcIlBhc3N3b3JkXCI6bnVsbCxcIlBhc3N3b3JkQnl0ZXNcIjpudWxsLFwiU3RhcnREYXRlXCI6XCIyMDIwLTEwLTIxVDE2OjI5OjAwXCIsXCJFbmREYXRlXCI6XCIyMDIwLTEwLTIxVDE2OjI5OjAwXCIsXCJBZG1pbkxldmVsXCI6MCxcIkF1dGhNb2RlXCI6MCxcIkVtYWlsXCI6bnVsbCxcIlR5cGVcIjoxLFwiRW50aXR5SWRcIjoxLFwiSXNBY3RpdmVcIjp0cnVlLFwiSXNBZG1pblwiOmZhbHNlLFwiUmVtYWluaW5nQ3JlZGl0XCI6MC4wLFwiQWxsb3dlZFN0b2NrQ291bnRcIjowLFwiRmluZ2VyVGVtcGxhdGVzXCI6W10sXCJGYWNlVGVtcGxhdGVzXCI6W10sXCJJZGVudGl0eUNhcmRcIjpudWxsfSIsImp0aSI6ImY4MWFiMzU5LTZlMjktNDVkZS1iZDNhLWFkNTEyMGRiMDFiNiIsImV4cCI6MTYzNTA3NDc4OH0.afI4VecgU50cQRhRwzae1JIVPlBd8wJpSsPt_aN-y44";
        public string KasraAdminToken { get; set; } = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VyIjoie1wiSWRcIjoxLFwiQ29kZVwiOjEyMzQ1Njc4OSxcIlVuaXF1ZUlkXCI6MTIzNDU2Nzg5LFwiVXNlck5hbWVcIjpcIkFkbWluXCIsXCJSZWdpc3RlckRhdGVcIjpcIjIwMjAtMTAtMjFUMTY6Mjk6MDBcIixcIkRlcGFydG1lbnROYW1lXCI6bnVsbCxcIlRlbE51bWJlclwiOm51bGwsXCJJbWFnZUJ5dGVzXCI6bnVsbCxcIkltYWdlXCI6bnVsbCxcIkZpcnN0TmFtZVwiOlwiQWRtaW5cIixcIlN1ck5hbWVcIjpcIkFkbWluaXN0cmF0b3JcIixcIkZ1bGxOYW1lXCI6XCIxX0FkbWluIEFkbWluaXN0cmF0b3JcIixcIlBhc3N3b3JkXCI6bnVsbCxcIlBhc3N3b3JkQnl0ZXNcIjpudWxsLFwiU3RhcnREYXRlXCI6XCIyMDIwLTEwLTIxVDE2OjI5OjAwXCIsXCJFbmREYXRlXCI6XCIyMDIwLTEwLTIxVDE2OjI5OjAwXCIsXCJBZG1pbkxldmVsXCI6MCxcIkF1dGhNb2RlXCI6MCxcIkVtYWlsXCI6bnVsbCxcIlR5cGVcIjoxLFwiRW50aXR5SWRcIjoxLFwiSXNBY3RpdmVcIjp0cnVlLFwiSXNBZG1pblwiOmZhbHNlLFwiUmVtYWluaW5nQ3JlZGl0XCI6MC4wLFwiQWxsb3dlZFN0b2NrQ291bnRcIjowLFwiRmluZ2VyVGVtcGxhdGVzXCI6W10sXCJGYWNlVGVtcGxhdGVzXCI6W10sXCJJZGVudGl0eUNhcmRcIjpudWxsfSIsImp0aSI6IjhjMDUxM2Y3LTk0NjktNDU2OS05ZjUzLTc0MjRiNDllNzQ2NSIsImV4cCI6MTYzNTA3NDc2OX0.Nb57Ig87eu87SPBmXv3CV9gAUMJrEa2Q9saHnt2HJj8";
        

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

            set => Configuration.GetSection("AppSettings")["MigrateUp"] = value.ToString();
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
                    return string.Equals(Configuration.GetSection("AppSettings")["LockDevice"], bool.TrueString, StringComparison.InvariantCultureIgnoreCase);
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
