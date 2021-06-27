using System.Diagnostics.CodeAnalysis;

namespace Biovation.Brands.Suprema.Model
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class EventModel
    {
        public int TypeID { get; set; }
        public int Data { get; set; }

    }
}
