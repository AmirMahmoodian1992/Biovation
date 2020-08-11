//using System.Linq;
//using System.Net;
//using System.Net.Sockets;
//using Biovation.CommonClasses.Manager;

//namespace Biovation.Brands.Virdi.Model
//{
//    /// <summary>
//    /// This class has some read-only configurational properties which is needed through the application.
//    /// It is constructed based on Singleton Pattern, for more information visit: https://sourcemaking.com/design_patterns/singleton
//    /// </summary>
//    public class AppConfig
//    {

//        public short ClientId;

//        private string _machineIpAddress;

//        private static AppConfig _instance;

//        /// <summary>
//        /// Return instantiated object of AppConfig class and instantiate it for first time.
//        /// </summary>
//        public static AppConfig Instance
//        {
//            get
//            {
//                if (_instance == null)
//                {
//                    _instance = new AppConfig();
//                    _instance.Initialize();
//                }
//                return _instance;
//            }
//        }

//        /// <summary>
//        /// Private Constructor for preventing instantiation from outside.
//        /// </summary>
//        private AppConfig()
//        {

//        }

//        /// <summary>
//        /// Initialize configuration from AppSetting object.
//        /// </summary>
//        public void Initialize()
//        {
//            var localIPAddresses = Dns.GetHostAddresses(Dns.GetHostName());
//            var localIP = localIPAddresses.First(x => x.AddressFamily == AddressFamily.InterNetwork);

//            _machineIpAddress = localIP.ToString();
//            ServerPort = BiovationConfigurationManager.VirdiDevicesConnectionPort;
//            //_commandServerIp = _appSetting["unis_command_server_ip"];
//            //_commandServerPort = int.Parse(_appSetting["unis_command_server_port"]);
//            //_udbServerIp = _appSetting["unis_udb_server_ip"];
//            //_udbServerPort = int.Parse(_appSetting["unis_udb_server_port"]);
//            //_unisDatabase = _appSetting["unis_database"];
//            //_unisDatabaseUsername = _appSetting["unis_database_username"];
//            //_unisDatabasePassword = _appSetting["unis_database_password"];
//            //_deviceCount = Convert.ToInt32(_appSetting["virdi_device_count"]);
//        }

//        public int ServerPort { get; private set; }

//        /// <summary>
//        /// The IP address of current running computer
//        /// </summary>
//        public string MachineIpAddress => _machineIpAddress;

//        /// <summary>
//        /// Version of Virdi AMS software
//        /// </summary>
//        public string Version { get; } = "Kasra AMS v1.0";
//    }
//}