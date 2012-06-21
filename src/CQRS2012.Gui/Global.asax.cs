using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using CQRS2012.Gui.Infrastructure;
using CQRS2012.Gui.Models;
using CQRS2012.Gui.Models.Database;
using CQRS2012.Gui.Models.Repositories;
using CQRS2012.Gui.Models.Services;

namespace CQRS2012.Gui
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

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            //autofac
            var builder = new ContainerBuilder();

            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<DateTimeForPolandProvider>().As<IDateTimeProvider>();
            builder.RegisterType<GameRepository>().As<IGameRepository>();
            builder.RegisterType<BetRepository>().As<IBetRepository>();
            builder.RegisterType<RankingRepository>().As<IRankingRepository>();
            builder.RegisterType<StrategyRepository>().As<IStrategyRepository>();
            builder.RegisterType<CommentRepository>().As<ICommentRepository>();
            builder.RegisterType<Scorer>().As<IScorer>();
            builder.RegisterType<StrategyFactory>().As<IStrategyFactory>();
            builder.RegisterType<BetStrategyRandom>().As<IBetStrategy>();
            builder.RegisterType<BetStrategyService>().As<IBetStrategyService>();
            builder.RegisterType<GameService>().As<IGameService>();
            builder.RegisterType<RankingService>().As<IRankingService>();
            builder.RegisterType<CommentService>().As<ICommentService>();

            IContainer container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            //

            //AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            FnHibernateHelper.BuildSessionFactory();

            var initialData = new InitialData(container.Resolve<IRankingService>(),container.Resolve<IBetStrategyService>());
            initialData.Setup();
        }

        //TODO: zamykanie sessionfactory
        protected void Application_End()
        {
            FnHibernateHelper.CloseSession();
        }

        protected void Application_EndRequest()
        {
        }
    }
}