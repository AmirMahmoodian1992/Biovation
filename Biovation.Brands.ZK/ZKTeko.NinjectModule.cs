using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biovation.CommonClasses;
using Biovation.CommonClasses.Interface;
using Ninject.Modules;

namespace Biovation.Brands.ZK
{
    public class ZKTecoNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IBrands>().To<ZKTeco>();
        }
    }
}
