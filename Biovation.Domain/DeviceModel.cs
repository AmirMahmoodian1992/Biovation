using DataAccessLayerCore.Attributes;

namespace Biovation.Domain
{
    public class DeviceModel
    {
        [Id]
        public int Id { get; set; }
        public string Name { get; set; }
        public int ManufactureCode { get; set; }
        [OneToOne]
        public Lookup Brand { get; set; }
        public int GetLogMethodType { get; set; }
        public string Description { get; set; }
        public int DefaultPortNumber{ get; set; }

    }

    public static class DeviceModels
    {
        public const int AC6000 = 1001;
        public const int AC2100 = 1002;
        public const int AC2200 = 1003;
        public const int STPro = 2001;
        public const int STProPlus = 2002;
        public const int Biomini = 3001;
        public const int BioLiteNet = 3002;
        public const int BioStation = 3003;
        public const int FaceStation = 3004;
        public const int BioStationT2 = 3005;
        public const int FaceG3 = 4001;
        public const int UFace202 = 4002;
        public const int UFace402 = 4003;
        public const int UFace602 = 4004;
        public const int IFace202 = 4005;
        public const int BlackWhite = 4006;
        public const int PW1100 = 5001;
        public const int PW1200 = 5002;
        public const int PW1400 = 5003;
        public const int PW1410 = 5004;
        public const int PW1500 = 5005;
        public const int PW1510 = 5006;
        public const int PW1520 = 5007;
        public const int PW1600 = 5008;
        public const int PW1650 = 5009;
        public const int PW1680 = 5010;
        public const int PW1700 = 5011;
        public const int MaxaClient = 6001;

        public static class ManufactureModels
        {
            public const int AC6000 = 6000;
            public const int AC2100 = 2100;
            public const int AC2200 = 10;
            public const int STPro = 0;
            public const int STProPlus = 0;
            public const int Biomini = 3001;
            public const int BioLiteNet = 2;
            public const int BioStation = 0;
            public const int FaceStation = 10;
            public const int BioStationT2 = 6;
            public const int FaceG3 = 0;
            public const int PW1100 = 1;
            public const int PW1200 = 2;
            public const int PW1400 = 3;
            public const int PW1410 = 4;
            public const int PW1500 = 5;
            public const int PW1510 = 6;
            public const int PW1520 = 7;
            public const int PW1600 = 8;
            public const int PW1650 = 9;
            public const int PW1680 = 10;
            public const int PW1700 = 11;
            public const int MaxaClient = 1;
        }
    }

    //public enum DeviceModels
    //{
    //   AC6000 = 1,
    //   AC2100 = 2
    //}
}
