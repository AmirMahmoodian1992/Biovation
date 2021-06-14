using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;

namespace Biovation.Brands.PFK.Managers
{
    public class DefaultDependencyResolver : System.Web.Mvc.IDependencyResolver, IDependencyResolver
    {
        protected IServiceProvider ServiceProvider;
        protected IServiceScope Scope;

        public DefaultDependencyResolver(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public DefaultDependencyResolver(IServiceScope scope)
        {
            Scope = scope;
            ServiceProvider = scope.ServiceProvider;
        }

        public object GetService(Type serviceType)
        {
            return ServiceProvider.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return ServiceProvider.GetServices(serviceType);
        }

        public IDependencyScope BeginScope()
        {
            return new DefaultDependencyResolver(ServiceProvider.CreateScope());
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            Scope?.Dispose();
        }
    }
}
