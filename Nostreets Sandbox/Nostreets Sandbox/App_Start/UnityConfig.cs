using Nostreets_Services.Interfaces.Services;
using Nostreets_Services.Services.Database;
using Nostreets_Services.Services.Email;
using NostreetsORM;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using NostreetsExtensions.Interfaces;

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

            //container.RegisterType<IMailChimpService, MailChimpService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IEmailService, /*SparkPostService*/SendGridService>(new ContainerControlledLifetimeManager(), new InjectionConstructor(WebConfigurationManager.AppSettings["SendGrid.ApiKey"]));
            container.RegisterType<IChartSrv, ChartsService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IBillService, BillService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IUserService, UserService>(new ContainerControlledLifetimeManager());

            ///Setting Resolvers for MVC and WebApi
            DependencyResolver.SetResolver(new UnityResolver(container));
            config.DependencyResolver = new UnityResolver(container);

            _container = container;
        }

    }

}