using System.Reflection;

namespace Biovation.Brands.ZK
{
    public class ZkTeco
    {

        /// <summary>
        /// Returns "ZKTeco"
        /// </summary>
        /// <returns></returns>
        public string GetBrandName()
        {
            return "ZKTeco";
        }

        public string GetBrandVersion() => Assembly.GetExecutingAssembly().GetName().Version?.ToString();
    }
}
