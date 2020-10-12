using System.Reflection;

namespace Biovation.Brands.Shahab
{
    public class Shahab
    {
        public string GetBrandName()
        {
            return "Shahab";
        }

        public string GetBrandVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
