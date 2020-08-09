
namespace Biovation.CommonClasses.Models
{
    public class AuthModeMap
    {
        public int Id { get; set; }
        public int BrandId { get; set; }
        public int AuthMode { get; set; }
        public string BioTitle { get; set; }
        public Lookup BioCode { get; set; }
    }
}
