using System.Reflection;

namespace Biovation.Brands.Virdi
{
    /// <summary>
    /// Main service for Virdi biometric devices. this class helps to intract with UNIS software and finally virdi bioclocks. 
    /// In order to use this service you have to install UNIS(at least UNIS Server, Command Server and UDB Server).
    /// </summary>
    public class Virdi/* : IBrands*/
    {
        private VirdiServer _virdiServer;

        public string GetBrandName()
        {
            return "Virdi";
        }

        public string GetBrandVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        /// <summary>
        /// Disconnect from UNIS server and UDB server.
        /// </summary>
        public void StopService()
        {
            _virdiServer.StopServer();
        }

        //public bool MigrateUp()
        //{
        //    return Migration.MigrateUp();
        //}
    }
}
