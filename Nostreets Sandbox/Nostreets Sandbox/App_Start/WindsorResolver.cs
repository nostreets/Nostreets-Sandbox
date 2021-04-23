using Castle.MicroKernel.Lifestyle;
using Castle.Windsor;
using Nostreets.Extensions.Extend.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;

namespace Nostreets_Sandbox.App_Start
{
    public class WindsorResolver : IDependencyResolver, System.Web.Mvc.IDependencyResolver
    {
        protected IWindsorContainer _container;

        public WindsorResolver(IWindsorContainer container)
        {
            _container = container ?? throw new ArgumentNullException("container");
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return _container.Resolve(serviceType);
            }
            catch (Exception ex)
            {
                ex.Message.LogInDebug();
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                List<object> result = new List<object>();
                foreach (object item in _container.ResolveAll(serviceType))
                    result.Add(item);
                return result;
            }
            catch (Exception ex)
            {
                ex.Message.LogInDebug();
                return new List<object>();
            }
        }

        public IDependencyScope BeginScope()
        {
            return new WindsorDependencyScope(_container);
        }

        public void Dispose()
        {
            _container.Dispose();
        }
    }

    public class WindsorDependencyScope : IDependencyScope
    {
        private readonly IWindsorContainer _container;
        private readonly IDisposable _scope;

        public WindsorDependencyScope(IWindsorContainer container)
        {
            _container = container;
            _scope = container.BeginScope();
        }

        public object GetService(Type serviceType)
        {
            if (_container.Kernel.HasComponent(serviceType))
                return _container.Resolve(serviceType);
            else
                return null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _container.ResolveAll(serviceType).Cast<object>();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}