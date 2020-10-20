using System.Reflection;

namespace Biovation.Brands.PW
{
    public class Pw
    {
        public string GetBrandName()
        {
            return "PW";
        }

        public string GetBrandVersion() => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}
