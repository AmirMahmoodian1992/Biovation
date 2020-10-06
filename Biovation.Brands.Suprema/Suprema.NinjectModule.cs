using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Ninject.Modules;

namespace Biovation.Brands.Suprema
{
    public class SupremaNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IBrands>().To<Suprema>();
        }
    }
}
