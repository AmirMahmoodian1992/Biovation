using System.Reflection;

namespace Biovation.Brands.Suprema
{
    /// <summary>
    /// کلاس اصلی ماژول دستگاه های سوپریما
    /// </summary>
    public class Suprema //: IBrands
    {
        //private readonly BioStarServer _bioStarServer;
        //private readonly FastSearchService _fastSearchService;


        //public Suprema(BioStarServer bioStarServer, FastSearchService fastSearchService)
        //{
        //    _bioStarServer = bioStarServer;
        //    _fastSearchService = fastSearchService;
        //}

        //public void StartService()
        //{
        //    //_bioStarServer.FactoryBioStarServer();

        //    Task.Run(() => _fastSearchService.Initial());

        //}

        //public void StopService()
        //{
        //    _bioStarServer.StopService();
        //}


        /// <summary>
        /// <En>Returns the module brand name</En>
        /// <Fa>نام برند ساعت را بر می گرداند</Fa>
        /// </summary>
        /// <returns></returns>
        public string GetBrandName()
        {
            return "Suprema";
        }

        public string GetBrandVersion()
        {
            var ret = Assembly.GetExecutingAssembly().GetName().Version;
            if (ret != null)
                return ret.ToString();
            return "";
        }
    }
}
