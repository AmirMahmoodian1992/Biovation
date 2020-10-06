using Biovation.Brands.Suprema.Commands;
using Biovation.Brands.Suprema.Service;
using System.Reflection;
using System.Threading.Tasks;

namespace Biovation.Brands.Suprema
{
    /// <summary>
    /// کلاس اصلی ماژول دستگاه های سوپریما
    /// </summary>
    public class Suprema //: IBrands
    {

        /// <summary>
        /// <En>Starts Suprema specific server</En>
        /// <Fa>سرور مخصوص به هر برند را استارت می کند</Fa>
        /// </summary>
        public void StartService()
        {
            BioStarServer.FactoryBioStarServer();

            Task.Run(() => FastSearchService.GetInstance().Initial());

        }

        public void StopService()
        {
            BioStarServer.FactoryBioStarServer().StopService();
        }

  
        /// <summary>
        /// <En>Returns the module brand name</En>
        /// <Fa>نام برند ساعت را بر می گرداند</Fa>
        /// </summary>
        /// <returns></returns>
        public string GetBrandName()
        {
            return "Suprema";
        }

        public string GetBrandVersion() => Assembly.GetExecutingAssembly().GetName().Version.ToString();


    }
}
