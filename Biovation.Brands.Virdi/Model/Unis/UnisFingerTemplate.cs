using DataAccessLayerCore.Attributes;

namespace Biovation.Brands.Virdi.Model.Unis
{
    public class UnisFingerTemplate
    {
        [Id]
        public int UserId { get; set; }
        public int IsWideChar { get; set; }
        public byte[] Template { get; set; }
    }
}
