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
                 Reg.Component.For<IDBService>().ImplementedBy<DBService>(),
                 Reg.Component.For(typeof(IDBService<>)).ImplementedBy(typeof(DBService<>)),
                 Reg.Component.For(typeof(IDBService<,>)).ImplementedBy(typeof(DBService<,>)),
                 Reg.Component.For(typeof(IDBService<,,,>)).ImplementedBy(typeof(DBService<,,,>)),
                 Reg.Component.For<IBillService>().ImplementedBy<BillService>()
                     .DependsOn((k, param) =>
                     {
                         param["incomeSrv"] = k.Resolve<IDBService<Income>>();
                         param["expenseSrv"] = k.Resolve<IDBService<Expense>>();
                         param["connectionString"] = "DefaultConnection";
                     }),
                 Reg.Component.For<IUserService>().ImplementedBy<UserService>()
                     .DependsOn((k, param) =>
                     {
                         param["emailSrv"] = k.Resolve<IDBService<Income>>();
                         param["userDBSrv"] = k.Resolve<IDBService<Expense>>();
                         param["tokenDBSrv"] = k.Resolve<IDBService<Expense>>();
                         param["connectionString"] = "DefaultConnection";
                     }),
                 Reg.Component.For<IChartService>().ImplementedBy<ChartsService>()
                     .DependsOn((k, param) =>
                     {
                         param["chartSrv"] = k.Resolve<IDBService<Income>>();
                         param["pieChartSrv"] = k.Resolve<IDBService<Expense>>();
                         param["connectionString"] = "DefaultConnection";
                     }),
                 Reg.Component.For<IEmailService>().ImplementedBy<SendGridService>()
                     .DependsOn((k, param) =>
                     {
                         param["apiKey"] = WebConfigurationManager.AppSettings["SendGrid.ApiKey"];
                     })
             );



        }
    }
}