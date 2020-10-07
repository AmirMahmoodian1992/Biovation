using System.Reflection;
using Biovation.CommonClasses;
using Biovation.Domain;

namespace Biovation.Brands.EOS
{
    /// <summary>
    /// کلاس اصلی ماژول دستگاه های علم و صنعت
    /// </summary>
    public class Eos
    {
        public string GetBrandName()
        {
            return "EOS";
        }

        public string GetBrandVersion() => Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}