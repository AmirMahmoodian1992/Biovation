using Biovation.CommonClasses.Models;
using System.Reflection;
using System.Web.Http;
using Biovation.CommonClasses.Interface;

namespace Biovation.Brands.ZK
{
    public class ZKTeco : IBrands
    {
        //private readonly ILogger _logger;
        private ZKTecoServer _zkTecoServer;

        /// <summary>
        /// Returns "ZKTeco"
        /// </summary>
        /// <returns></returns>
        public string GetBrandName()
        {
            return "ZKTeco";
        }

        public string GetBrandVersion() => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        /// <summary>
        /// <En>Deserialise JSON received data from socket connection ( that has been sent from BiovationServer )  and send request to EventHandler</En>
        /// <Fa>داده ی دریافت شده از اتصال سوکت را که از BiovationServer دریافت کرده ، از فرمت JSON خارج کرده و به EventHandler ارسال میکند</Fa>
        /// </summary>
        /// <param name="receivedData"></param>
        public void EventPasser(DataTransferModel receivedData)
        {

        }

        public void EventPasser(DataTransferModel receivedData, object senderSocket)
        {
        }

        /// <summary>
        /// Starting service includes: 
        ///     1-UNIS server connection. 
        ///     2-UNIS server authentication.
        ///     3-UDB server connection.
        ///     4-UDB server authentication. 
        /// </summary>
        public void StartService()
        {
            _zkTecoServer = ZKTecoServer.FactoryZKServer();
            _zkTecoServer.StartServer();
        }

        /// <summary>
        /// Disconnect from UNIS server and UDB server.
        /// </summary>
        public void StopService()
        {
            _zkTecoServer.StopServer();
        }

        public void RegisterArea(object httpConfiguration)
        {
            var config = (HttpConfiguration)httpConfiguration;
            //config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "ZKTecoApi",
                routeTemplate: "Biovation/Api/ZK/{controller}/{action}"
            );
        }

        public bool MigrateUp()
        {
            return Migration.MigrateUp();
        }
    }
}
