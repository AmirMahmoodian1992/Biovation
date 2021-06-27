using System.Collections.Generic;
using DataAccessLayerCore.Attributes;
using DataAccessLayerCore.Domain;

namespace Biovation.Domain
{
    public class DeviceBrand
    {
        [Id]
        public string Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        [OneToMany(FetchType = FetchType.EAGER)]
        public List<DeviceModel> Models { get; set; }
    }
    
    //public static class DeviceBrands
    //{
    //    public const int Virdi = 1;
    //    public const int EOS = 2;
    //    public const int Suprema = 3;
    //    public const int ZKTeco = 4;
    //    public const int PW = 5;
    //    public const int Maxa = 6;
    //}
}
