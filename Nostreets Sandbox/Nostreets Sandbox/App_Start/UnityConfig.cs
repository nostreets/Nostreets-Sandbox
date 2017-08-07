using Microsoft.Practices.Unity;
using Nostreets_Services.Domain.Cards;
using Nostreets_Services.Interfaces.Services;
using Nostreets_Services.Providers;
using Nostreets_Services.Services.Database;
using Nostreets_Services.Services.Web;
using NostreetsORM;
using NostreetsORM.Interfaces;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using Unity.Mvc5;

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
            container.RegisterType<IChartsExtended, ChartsService>(new ContainerControlledLifetimeManager(), new InjectionConstructor("AzureDBConnection"/*"DefaultConnection"*/));
            container.RegisterType<IDBService<StyledCard>, DBService<StyledCard>>(new ContainerControlledLifetimeManager(), new InjectionConstructor("AzureDBConnection"/*"DefaultConnection"*/));
            container.RegisterType<IUserService, UserService>(new ContainerControlledLifetimeManager(), new InjectionConstructor());


            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            config.DependencyResolver = new UnityResolver(container);
            _container = container;
        }

    }
}