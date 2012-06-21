using System;
using System.Data.SqlClient;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace CQRS2012.Gui.Models.Database
{
    public class FnHibernateHelper
    {
        private static ISessionFactory _sessionFactory;
        private static Configuration _configuration;

        public static void BuildSessionFactory()
        {

            AutoPersistenceModel model = CreateMappings();

            _configuration = Fluently.Configure()
//                                    .Database(MsSqlConfiguration.MsSql2008.ConnectionString(c => c.FromConnectionStringWithKey("MyConnectionString")))
//                                      .Database(MsSqlCeConfiguration.Standard.ConnectionString(c => c.FromConnectionStringWithKey("MyConnectionString")))
                                    .Database(SQLiteConfiguration.Standard.ConnectionString(c => c.FromConnectionStringWithKey("MyConnectionString")))
//                                    .Database(MsSqlConfiguration.MsSql2008.ConnectionString("server=(local)\\SQLEXPRESS;database=CQRS2012;uid=test;pwd=test1"))
//                                    .Database(MsSqlCeConfiguration.Standard.ConnectionString("Data Source=DBTest2012.sdf"))
//                                    .Database(MsSqlCeConfiguration.Standard.ConnectionString(c => c.FromConnectionStringWithKey("MyConnectionString")))
                                     .Mappings(m => m.AutoMappings.Add(model))
                                     .ExposeConfiguration(BuildSchema)
                                     .BuildConfiguration();

            _sessionFactory = _configuration.BuildSessionFactory();
        }

        public static ISessionFactory SessionFactory { get { return _sessionFactory; } }

        public static AutoPersistenceModel CreateMappings()
        {
            var cfg = new DbMapConfiguration();
            
            return AutoMap.Assemblies(cfg, AppDomain.CurrentDomain.GetAssemblies())
                          .Conventions.Add(DefaultLazy.Never());
            //.Where(t => t.Namespace == "CQRS2012");
        }

        public static void BuildSchema(Configuration config)
        {
            new SchemaUpdate(config).Execute(true, true);
            //  new SchemaExport(config).Create(true, true);
        }

        public static void CloseSession()
        {
            if (_sessionFactory != null)
            {
                _sessionFactory.Close();
            }
        }

        public static void InTransaction(Action<ISession> action)
        {
            using (var session = SessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    action(session);
                    tx.Commit();
                }
            }
        }

        public static object InTransaction(Func<ISession, object> action)
        {
            object obj;
            using (var session = SessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    obj = action(session);
                    tx.Commit();
                }
            }

            return obj;
        }

    }
}