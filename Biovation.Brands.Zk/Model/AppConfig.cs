using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Biovation.Brands.ZK.Model {
    /// <summary>
    /// This class has some read-only configurational properties which is needed through the application.
    /// It is constructed based on Singleton Pattern, for more information visit: https://sourcemaking.com/design_patterns/singleton
    /// </summary>
    public class AppConfig {

        public short ClientId;

        private NameValueCollection _appSetting;

        private int _serverPort;

        private string _machineIpAddress;
        private string _commandServerIp;
        private int _commandServerPort;
        private int _deviceCount;
        private string _udbServerIp;
        private int _udbServerPort;
        private string _version = "Kasra AMS v1.0";
        private string _unisDatabase;
        private string _unisDatabaseUsername;
        private string _unisDatabasePassword;

        private static AppConfig _instance;

        /// <summary>
        /// Return instantiated object of AppConfig class and instantiate it for first time.
        /// </summary>
        public static AppConfig Instance {
            get {
                if (_instance == null) {
                    _instance = new AppConfig();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Private Constructor for preventing instantiation from outside.
        /// </summary>
        private AppConfig() {

        }

        /// <summary>
        /// Initialize configuration from AppSetting object.
        /// </summary>
        /// <param name="appSetting"></param>
        public void Initialize(NameValueCollection appSetting) {
            _appSetting = appSetting;

            var localIPAddresses = Dns.GetHostAddresses(Dns.GetHostName());
            var localIP = localIPAddresses.First(x => x.AddressFamily == AddressFamily.InterNetwork);
            
            _machineIpAddress = localIP.ToString();
            _serverPort = Convert.ToInt32(appSetting["Zk_server_port"]);
            _commandServerIp = appSetting["unis_command_server_ip"];
            _commandServerPort = int.Parse(appSetting["unis_command_server_port"]);
            _udbServerIp = appSetting["unis_udb_server_ip"];
            _udbServerPort = int.Parse(appSetting["unis_udb_server_port"]);
            _unisDatabase = appSetting["unis_database"];
            _unisDatabaseUsername = appSetting["unis_database_username"];
            _unisDatabasePassword = appSetting["unis_database_password"];
            _deviceCount = Convert.ToInt32(appSetting["Zk_device_count"]);
        }

        public int ServerPort { get { return _serverPort; } }

        public int DeviceCount { get { return _deviceCount; } }

        /// <summary>
        /// The IP address of current running computer
        /// </summary>
        public string MachineIpAddress { get { return _machineIpAddress; } }
        /// <summary>
        /// UNIS command server IP address. 127.0.0.1 by default.
        /// </summary>
        public string CommandServerIp { get { return _commandServerIp; } }
        /// <summary>
        /// UNIS command server Port number.9871 by default.
        /// </summary>
        public int CommandServerPort { get { return _commandServerPort; } }
        /// <summary>
        /// UNIS database server IP address. 127.0.0.1 by default.
        /// </summary>
        public string UdbServerIp { get { return _udbServerIp; } }
        /// <summary>
        /// UNIS database server port number. 9872 by default.
        /// </summary>
        public int UdbServerPort { get { return _udbServerPort; } }

        /// <summary>
        /// Version of Zk AMS software
        /// </summary>
        public string Version { get { return _version; } }

        /// <summary>
        /// Name of prepared UNIS database. (It Define in ODBC Data Source)
        /// </summary>
        public string UnisDatabase { get { return _unisDatabase; } }

        /// <summary>
        /// Username for accessing to prepared UNIS database. (It Define in ODBC Data Source)
        /// </summary>
        public string UnisDatabaseUsername { get { return _unisDatabaseUsername; } }

        /// <summary>
        /// Password for accessing to prepared UNIS database. (It Define in ODBC Data Source)
        /// </summary>
        public string UnisDatabasePassword { get { return _unisDatabasePassword; } }

    }
}