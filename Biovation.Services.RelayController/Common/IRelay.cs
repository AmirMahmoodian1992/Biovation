using Biovation.Domain.RelayModels;

namespace Biovation.Services.RelayController.Common
{
    public interface IRelay
    {
        public Relay RelayInfo { get; set; }

        public bool Connect();
        public bool Disconnect();
        public bool IsConnected();
        public bool Contact();
        public bool TurnOn();
        public bool TurnOff();
        public bool FlashOn();
        public bool FlashOff();
        public string GetData();
        public string GetStatus();
        //public bool SendData(string data);
        //public bool SendQuerry(string querry);
        //public string ReadData();
        //public string GetSerializedDeviceInfo();
        //public string GetDeviceInfo();
        //public string GetRelayStatus();
        //public string GetInputStatus();
        //public string StringRevers(string str);
    }
}