using System.Reflection;

namespace Biovation.Brands.Paliz
{
    public class Paliz
    {
        /// <summary>
        /// Returns "Paliz"
        /// </summary>
        /// <returns></returns>
        public string GetBrandName()
        {
            return "Paliz";
        }

        public string GetBrandVersion() => Assembly.GetExecutingAssembly().GetName().Version?.ToString();
    }
}
