using Nostreets_Services.Domain.Cards;
using Nostreets_Services.Interfaces.Services;
using Nostreets_Services.Services.Database;
using Nostreets_Services.Services.Web;
using NostreetsEntities;
using NostreetsORM;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using Unity.Mvc5;
using System;
using System.Net.Http;
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

            //container.RegisterType<>

            container.RegisterType<IMailChimpService, MailChimpService>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISendGridService, SendGridService>(new ContainerControlledLifetimeManager(), new InjectionConstructor(WebConfigurationManager.AppSettings["SendGrid.ApiKey"]));
            container.RegisterType<IChartsExtended, ChartsService>(new ContainerControlledLifetimeManager(), new InjectionConstructor(/*"AzureDBConnection"*/"DefaultConnection"));
            container.RegisterType<IDBService<StyledCard>, DBService<StyledCard>>(new ContainerControlledLifetimeManager(), new InjectionConstructor(/*"AzureDBConnection"*/"DefaultConnection"));
            container.RegisterType<IUserService, UserService>(new ContainerControlledLifetimeManager(), new InjectionConstructor(/*"AzureDBConnection"*/"DefaultConnection"));
            container.RegisterType<IBillService, BillService>(new ContainerControlledLifetimeManager(), new InjectionConstructor(/*"AzureDBConnection"*/"DefaultConnection"));

            container.RegisterType(typeof(IDBService<>), typeof(DBService<>), new ContainerControlledLifetimeManager(), new InjectionConstructor(/*"AzureDBConnection"*/"DefaultConnection"));
            container.RegisterType(typeof(IEFDBService<>), typeof(EFDBService<>), new ContainerControlledLifetimeManager(), new InjectionConstructor(/*"AzureDBConnection"*/"DefaultConnection"));

            ///Setting Resolvers for MVC and WebApi
            DependencyResolver.SetResolver(new UnityResolver(container));
            config.DependencyResolver = new UnityResolver(container);

            _container = container;
        }

    }

    //public class UnityHttpControllerActivator : IHttpControllerActivator
    //{
    //    private IUnityContainer _container;

    //    public UnityHttpControllerActivator(IUnityContainer container)
    //    {
    //        _container = container;
    //    }

    //    public IHttpController Create(HttpControllerContext controllerContext, Type controllerType)
    //    {
    //        return (IHttpController)_container.Resolve(controllerType);
    //    }

    //    public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
    //    {
    //        return (IHttpController)_container.Resolve(controllerType);
    //    }
    //}
}