using Nostreets_Services.Domain.Cards;
using Nostreets_Services.Interfaces.Services;
using Nostreets_Services.Services.Database;
using Nostreets_Services.Services.Web;
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

            container.RegisterType<IMailChimpService, MailChimpService>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISendGridService, SendGridService>(new ContainerControlledLifetimeManager(), new InjectionConstructor(WebConfigurationManager.AppSettings["SendGrid.ApiKey"]));
            container.RegisterType<IChartSrv, ChartsService>(new ContainerControlledLifetimeManager(), new InjectionConstructor(/*"AzureDBConnection"*/"DefaultConnection"));
            //container.RegisterType<IDBService<StyledCard>, DBService<StyledCard>>(new ContainerControlledLifetimeManager(), new InjectionConstructor(/*"AzureDBConnection"*/"DefaultConnection"));
            container.RegisterType<IUserService, UserEFService>(new ContainerControlledLifetimeManager(), new InjectionConstructor(/*"AzureDBConnection"*/"DefaultConnection"));
            container.RegisterType<IBillService, BillService>(new ContainerControlledLifetimeManager(), new InjectionConstructor(/*"AzureDBConnection"*/"DefaultConnection"));

            container.RegisterType(typeof(IDBService<>), typeof(DBService<>), new ContainerControlledLifetimeManager(), new InjectionConstructor(/*"AzureDBConnection"*/"DefaultConnection"));
            //container.RegisterType(typeof(IDBService<>), typeof(EFDBService<>), new ContainerControlledLifetimeManager(), new InjectionConstructor(/*"AzureDBConnection"*/"DefaultConnection"));

            ///Setting Resolvers for MVC and WebApi
            DependencyResolver.SetResolver(new UnityResolver(container));
            config.DependencyResolver = new UnityResolver(container);

            _container = container;
        }

    }

}