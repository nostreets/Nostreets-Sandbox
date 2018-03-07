using Reg = Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using NostreetsExtensions.Utilities;
using Castle.Windsor.Installer;
using NostreetsExtensions.Interfaces;
using NostreetsORM;
using Nostreets_Services.Domain.Bills;
using Nostreets_Services.Interfaces.Services;
using Nostreets_Services.Services.Database;
using Nostreets_Services.Services.Email;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Http;
using Nostreets_Services.Domain.Cards;

namespace Nostreets_Sandbox.App_Start
{
    public class WindsorConfig : Disposable
    {

        public static void RegisterInterfaces(HttpConfiguration config)
        {
            WindsorContainer container = new WindsorContainer();
            container.Install(FromAssembly.This());


            DependencyResolver.SetResolver(new WindsorResolver(container));
            config.DependencyResolver = new WindsorResolver(container);


            _container = container;
        }

        public static WindsorContainer GetContainer()
        {
            return _container;
        }

        private static WindsorContainer _container;

    }

    public class RepositoriesInstaller : Reg.IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                 Reg.Component.For<IDBService>().ImplementedBy<DBService>().LifestyleSingleton(),
                 Reg.Component.For(typeof(IDBService<>)).ImplementedBy(typeof(DBService<>)).LifestyleSingleton(),
                 Reg.Component.For(typeof(IDBService<,>)).ImplementedBy(typeof(DBService<,>)).LifestyleSingleton(),
                 Reg.Component.For(typeof(IDBService<,,,>)).ImplementedBy(typeof(DBService<,,,>)).LifestyleSingleton(),
                 Reg.Component.For<IBillService>().ImplementedBy<BillService>().LifestyleSingleton()
                     .DependsOn((k, param) =>
                     {
                         param["incomeSrv"] = k.Resolve<IDBService<Income>>();
                         param["expenseSrv"] = k.Resolve<IDBService<Expense>>();
                         param["connectionString"] = "DefaultConnection";
                     }),
                 Reg.Component.For<IUserService>().ImplementedBy<UserService>().LifestyleSingleton()
                     .DependsOn((k, param) =>
                     {
                         param["emailSrv"] = k.Resolve<IDBService<Income>>();
                         param["userDBSrv"] = k.Resolve<IDBService<Expense>>();
                         param["tokenDBSrv"] = k.Resolve<IDBService<Expense>>();
                     }),
                 Reg.Component.For<IChartService>().ImplementedBy<ChartsService>().LifestyleSingleton()
                     .DependsOn((k, param) =>
                     {
                         param["chartSrv"] = k.Resolve<IDBService<Income>>();
                         param["pieChartSrv"] = k.Resolve<IDBService<Expense>>();
                         param["connectionString"] = "DefaultConnection";
                     }),
                 Reg.Component.For<IEmailService>().ImplementedBy<SendGridService>().LifestyleSingleton()
                     .DependsOn((k, param) =>
                     {
                         param["apiKey"] = WebConfigurationManager.AppSettings["SendGrid.ApiKey"];
                     })
             );


            container.Register(
                Reg.Component.For<Controllers.Api.SandoxApiController>().LifestyleSingleton()
                     .DependsOn((k, param) =>
                     {
                         param["emailSrv"] = k.Resolve<IEmailService>();
                         param["billSrv"] = k.Resolve<IBillService>();
                         param["userSrv"] = k.Resolve<IUserService>();
                         param["cardSrv"] = k.Resolve<IChartService>();
                         param["cardSrv"] = k.Resolve<IDBService<StyledCard>>();
                     })
            );



        }
    }
}