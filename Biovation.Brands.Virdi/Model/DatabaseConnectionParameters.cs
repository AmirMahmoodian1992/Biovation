using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biovation.Brands.Virdi.Model
{
    public class DatabaseConnectionParameters
    {
        public string Server { get; set; }
        public string DatabaseName { get; set; }
        public string Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
