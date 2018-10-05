using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Castle.MicroKernel;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Nostreets_Sandbox.Classes;
using Nostreets_Services.Domain;
using Nostreets_Services.Domain.Bills;
using Nostreets_Services.Domain.Charts;
using Nostreets_Services.Interfaces.Services;
using Nostreets_Services.Models.Request;
using Nostreets_Services.Services.Database;
using Nostreets_Services.Services.Email;
using NostreetsEntities;
using NostreetsExtensions.DataControl.Classes;
using NostreetsExtensions.Interfaces;
using NostreetsExtensions.Utilities;
using NostreetsORM;
using Reg = Castle.MicroKernel.Registration;

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
            #region ORMOptions Func
            ORMOptions ormOptions(IKernel k) =>
                   new ORMOptions
                   {
                       ConnectionKey = ConfigKeys.DBConnectionKey,
                       ErrorLog = k.Resolve<IDBService<Error>>(),
                       WipeDB = false,
                       NullLock = false
                   }; 
            #endregion

            container.Register(

                Reg.Component.For(typeof(IDBService<Error>)).ImplementedBy(typeof(EFDBService<Error>)).LifestyleSingleton()
                    .DependsOn((k, param) =>
                     {
                         param["connectionKey"] = ConfigKeys.DBConnectionKey;
                     }),

                Reg.Component.For(typeof(IDBService<>)).ImplementedBy(typeof(DBService<>)).LifestyleSingleton()
                    .DependsOn((k, param) =>
                     {
                         param["options"] = ormOptions(k);
                     }),

                 Reg.Component.For(typeof(IDBService<,>)).ImplementedBy(typeof(DBService<,>)).LifestyleSingleton()
                    .DependsOn((k, param) =>
                    {
                        param["options"] = ormOptions(k);
                    }),

                 Reg.Component.For(typeof(IDBService<,,,>)).ImplementedBy(typeof(DBService<,,,>)).LifestyleSingleton()
                    .DependsOn((k, param) =>
                     {
                         param["options"] = ormOptions(k);
                     }),

                 Reg.Component.For<IBillService>().ImplementedBy<BillService>().LifestyleSingleton()
                     .DependsOn((k, param) =>
                     {
                         param["incomeSrv"] = k.Resolve<IDBService<Income>>();
                         param["expenseSrv"] = k.Resolve<IDBService<Expense>>();
                     }),

                 Reg.Component.For<IChartService>().ImplementedBy<ChartsService>().LifestyleSingleton()
                     .DependsOn((k, param) =>
                     {
                         param["charDBtSrv"] = k.Resolve<IDBService<ChartistChart<List<int>>, int, ChartAddRequest, ChartUpdateRequest>>();
                         param["pieDBChartSrv"] = k.Resolve<IDBService<ChartistChart<int>, int, ChartAddRequest<int>, ChartUpdateRequest<int>>>();
                     }),

                 Reg.Component.For<IEmailService>().ImplementedBy<SendGridService>().LifestyleSingleton()
                     .DependsOn((k, param) =>
                     {
                         param["apiKey"] = ConfigKeys.SendGridApiKey;
                         param["errorLog"] = k.Resolve<IDBService<Error>>();
                     }),

                  Reg.Component.For<IUserService>().ImplementedBy<UserService>().LifestyleSingleton()
                     .DependsOn((k, param) =>
                     {
                         param["context"] = HttpContext.Current;
                         param["emailSrv"] = k.Resolve<IEmailService>();
                         param["userDBSrv"] = k.Resolve<IDBService<User, string>>();
                         param["tokenDBSrv"] = k.Resolve<IDBService<Token, string>>();
                     })
             );
        }
    }
}