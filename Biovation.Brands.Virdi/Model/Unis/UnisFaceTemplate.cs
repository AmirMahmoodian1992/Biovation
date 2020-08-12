using DataAccessLayerCore.Attributes;

namespace Biovation.Brands.Virdi.Model.Unis
{
    public class UnisFaceTemplate
    {
        [Id]
        public int UserId { get; set; }
        public byte[] Template { get; set; }
    }
}
