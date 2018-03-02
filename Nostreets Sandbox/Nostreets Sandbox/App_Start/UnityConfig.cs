
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Collections.Generic;
using Unity;
using Unity.Lifetime;
using Unity.Injection;
using NostreetsORM;
using NostreetsExtensions;
using NostreetsExtensions.Interfaces;
using Nostreets_Services.Domain.Charts;
using Nostreets_Services.Domain;
using Nostreets_Services.Domain.Bills;
using Nostreets_Services.Domain.Base;
using Nostreets_Services.Models.Request;
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

            //container.RegisterType<IMailChimpService, MailChimpService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IEmailService, /*SparkPostService*/SendGridService>(new ContainerControlledLifetimeManager(), new InjectionConstructor(WebConfigurationManager.AppSettings["SendGrid.ApiKey"]));
            container.RegisterType<IChartSrv, ChartsService>(new ContainerControlledLifetimeManager(), 
                                                             new InjectionConstructor(
                                                                 typeof(IDBService<Chart<List<int>>, int, ChartAddRequest, ChartUpdateRequest>).UnityResolve(container),
                                                                 typeof(IDBService<Chart<int>, int, ChartAddRequest<int>, ChartUpdateRequest<int>>).UnityResolve(container),
                                                                " DefaultConnection")
                                                            );
            container.RegisterType<IBillService, BillService>(new ContainerControlledLifetimeManager(),
                                                             new InjectionConstructor(
                                                                 (IDBService<Income>)typeof(IDBService<Income>).UnityResolve(container),
                                                                 typeof(IDBService<Expense>).UnityResolve(container),
                                                                " DefaultConnection")
                                                            );
            container.RegisterType<IUserService, UserService>(new ContainerControlledLifetimeManager(),
                                                             new InjectionConstructor(
                                                                 typeof(IEmailService).UnityResolve(container),
                                                                 typeof(IDBService<Token>).UnityResolve(container),
                                                                 typeof(IDBService<User>).UnityResolve(container),
                                                                " DefaultConnection")
                                                            );

            ///Setting Resolvers for MVC and WebApi
            DependencyResolver.SetResolver(new UnityResolver(container));
            config.DependencyResolver = new UnityResolver(container);

            _container = container;
        }

    }

}