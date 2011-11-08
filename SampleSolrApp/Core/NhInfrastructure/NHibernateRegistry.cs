using System;

using System.Configuration;
using System.IO;

using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;

using SampleSolrApp.Models.Nh;
using SampleSolrApp.Core.Repository;
using SampleSolrApp.Core.Ioc;


using NHibernate;

using NHibernate.Event;
using NHibernate.Event.Default;
using NHibernate.Search.Event;
using NHibernate.Search.Store;

using NHibernate.SolrNet;
using NHibernate.SolrNet.Impl;
using SolrNet.Mapping;


using StructureMap;
using StructureMap.Configuration.DSL;
using Environment = NHibernate.Cfg.Environment;

namespace SampleSolrApp.Core.NhInfrastructure
{
    public class NHibernateRegistry : Registry
    {

        public NHibernateRegistry()
        {
            var cfg = new NHibernate.Cfg.Configuration();

            IPersistenceConfigurer database = MsSqlConfiguration.MsSql2005.ConnectionString(ConfigurationManager.ConnectionStrings["SampleSolrApp"].ConnectionString);

            Fluently.Configure()
                .Database(database)
                .Mappings(m => m.HbmMappings.AddFromAssemblyOf<Product>())
                .ExposeConfiguration(c =>
                {
                    cfg = c;
                })
                .BuildConfiguration();

            // Solr & NHibernate integration ->
            var cfgHelper = new NHibernate.SolrNet.CfgHelper();
            cfgHelper.Configure(cfg, true);
            // <- Solr & NHibernate integration

            // manual NHibernate integration -> 
            //cfg.SetListener(ListenerType.PostCommitInsert, new SolrEventListener());
            //cfg.SetListener(ListenerType.PostCommitUpdate, new SolrEventListener());
            //cfg.SetListener(ListenerType.PostCommitDelete, new SolrEventListener());
            // <- manual NHibernate integration 

            var sessionFactory = cfg.BuildSessionFactory();

            For<NHibernate.Cfg.Configuration>().Singleton().Use(cfg);

            For<ISessionFactory>().Singleton().Use(sessionFactory);
            For<ISession>().HybridHttpOrThreadLocalScoped().Use(ctx => ctx.GetInstance<ISessionFactory>().OpenSession());
            For<IUnitOfWork>().HybridHttpOrThreadLocalScoped().Use<UnitOfWork>();
            For<ISession>().HybridHttpOrThreadLocalScoped().Use(ctx => ctx.GetInstance<IUnitOfWork>().CurrentSession);
            For<INhRepository>().HybridHttpOrThreadLocalScoped().Use<SampleSolrApp.Core.Repository.Repository>();
        }
    }
}