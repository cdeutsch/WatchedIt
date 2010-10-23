using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using Ninject.Modules;
using Site.Infrastructure.Logging;
using Web.Infrastructure.Authentication;
using Web.Models;
using Mvc3Ninject.Utility;
using Web.Reporting;
using System.Data.Entity.Infrastructure;
using Web.Infrastructure.Session;
namespace Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");
            routes.MapRoute(
                "Login", // Route name
                "login", // URL with parameters
                new { controller = "Session", action = "Create" } // Parameter defaults
            );
            routes.MapRoute(
                "Logout", // Route name
                "logout", // URL with parameters
                new { controller = "Session", action = "Delete" } // Parameter defaults
            );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        public static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<IControllerFactory>().To<DefaultControllerFactory>();

            //a typical binding
            kernel.Bind<ILogger>().To<NLogLogger>().InSingletonScope();
            //Bind<INoSqlServer>().To<DB4OServer>().InSingletonScope();
            //Bind<ISession>().To<Db4oSession>().InRequestScope();

            //You can use the SimpleRepository to build out your database
            //it runs "Auto Migrations" - changing your schema on the fly for you
            //should you change your model. You can switch it out as you need.
            //http://subsonicproject.com/docs/Using_SimpleRepository
            kernel.Bind<IAuthenticationService>().To<UserAuthenticationService>();
            kernel.Bind<IUserSession>().To<WebUserSession>();
        }

        public void SetupDependencyInjection()
        {
            // Create Ninject DI Kernel 
            _container = new StandardKernel();

            //// Register services with our Ninject DI Container
            RegisterServices(_container);

            //// Tell ASP.NET MVC 3 to use our Ninject DI Container 
            DependencyResolver.SetResolver(new NinjectResolver(_container));
        }

        void Application_Start()
        {
            //Database.SetInitializer<SiteDB>(new AlwaysRecreateDatabase<SiteDB>());
            //Database.SetInitializer<ReportingDB>(new AlwaysRecreateDatabase<ReportingDB>());
            
            //Database.SetInitializer<SiteDB>(new RecreateDatabaseIfModelChanges<SiteDB>());
            Database.SetInitializer<SiteDB>(new SiteDBTestInitializer(new UserAuthenticationService())); //seed test data
            Database.SetInitializer<ReportingDB>(new RecreateDatabaseIfModelChanges<ReportingDB>());

            SetupDependencyInjection();

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            Logger.Info("App is starting");
        }

        protected void Application_End()
        {
            Logger.Info("App is shutting down");
        }

        protected void Application_Error()
        {
            Exception lastException = Server.GetLastError();
            Logger.Fatal(lastException);
        }

        public ILogger Logger
        {
            get
            {
                return Container.Get<ILogger>();
            }
        }

        static IKernel _container;
        public static IKernel Container
        {
            get
            {
                return _container;
            }
        }

    }
}