using DataAccessLayerCore.Attributes;

namespace Biovation.Brands.Virdi.Model.Unis
{
    public class UnisFaceWalkThroughTemplate
    {
        [Id]
        public byte[] Data { get; set; }
        public int Type { get; set; }
        public int Length { get; set; }
    }
}
