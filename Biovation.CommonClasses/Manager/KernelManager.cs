using Ninject;
using Biovation.CommonClasses.Interface;

namespace Biovation.CommonClasses.Models
{
    public static class KernelManager
    {
        public static IBrands[] ModulesArray;
        public static StandardKernel Kernel;
    }
}