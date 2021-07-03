using System;

namespace Biovation.Domain
{
    public class Connection
    {
        public string Ip
        {
            get => _ip;
            set
            {
                try
                {
                    var parseResult = IPv4.TryParse(value, out var parsedIp);
                    if (parseResult)
                        ParsedIp = parsedIp;
                }
                catch (Exception)
                {
                    //ignore
                }

                _ip = value;
            }
        }

        private string _ip;
        public IPv4 ParsedIp { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string MacAddress { get; set; }
    }
}