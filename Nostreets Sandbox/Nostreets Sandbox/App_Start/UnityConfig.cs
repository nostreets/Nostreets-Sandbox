
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using Unity;
using Unity.Lifetime;
using Unity.Injection;
using Nostreets.Orm.Ado;
using NostreetsExtensions.Interfaces;
using Nostreets_Services.Interfaces.Services;
using Nostreets_Services.Services.Database;
using Nostreets_Services.Services.Email;

namespace Nostreets_Sandbox.App_Start
{
    public class UnityConfig
    {
        private static UnityContainer _container;

        public static UnityContainer GetContainer()
        {
            return _container;
        }

        public static void RegisterInterfaces(HttpConfiguration config)
        {
            UnityContainer container = new UnityContainer();

            container.RegisterType(typeof(IDBService<>), typeof(DBService<>), new ContainerControlledLifetimeManager(), new InjectionConstructor(/*"AzureDBConnection"*/"DefaultConnection"));
            container.RegisterType(typeof(IDBService<,>), typeof(DBService<,>), new ContainerControlledLifetimeManager(), new InjectionConstructor(/*"AzureDBConnection"*/"DefaultConnection"));
            container.RegisterType(typeof(IDBService<,,,>), typeof(DBService<,,,>), new ContainerControlledLifetimeManager(), new InjectionConstructor(/*"AzureDBConnection"*/"DefaultConnection"));
            container.RegisterType<IEmailService, /*SparkPostService*/SendGridService>(new ContainerControlledLifetimeManager(), new InjectionConstructor(WebConfigurationManager.AppSettings["SendGrid.ApiKey"]));
            container.RegisterType<IChartService, ChartsService>(new ContainerControlledLifetimeManager(),new InjectionConstructor());
            container.RegisterType<IBillService, BillService>(new ContainerControlledLifetimeManager(),new InjectionConstructor());
            container.RegisterType<IUserService, UserService>(new ContainerControlledLifetimeManager(),new InjectionConstructor());

            container.RegisterType<InterceptorConfig>(new ContainerControlledLifetimeManager(), new InjectionConstructor());
            container.RegisterType<Controllers.Api.SandoxApiController>(new ContainerControlledLifetimeManager(), new InjectionConstructor());
            container.RegisterType<Controllers.HomeController>(new ContainerControlledLifetimeManager(), new InjectionConstructor());

            ///Setting Resolvers for MVC and WebApi
            DependencyResolver.SetResolver(new UnityResolver(container));
            config.DependencyResolver = new UnityResolver(container);

            _container = container;
        }

    }

}