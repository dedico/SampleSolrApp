using System.Configuration;

using SampleSolrApp.Models.Nh;
using SampleSolrApp.Core.NhInfrastructure;


using StructureMap;
using StructureMap.Pipeline;
using StructureMap.Util;

using StructureMap.SolrNetIntegration;
using StructureMap.SolrNetIntegration.Config;

using SolrNet;
using SolrNet.Impl;
using SolrNet.Mapping;

using NHibernate.SolrNet;

using Microsoft.Practices.ServiceLocation;



namespace SampleSolrApp.Core.Ioc
{
    public class ContainerBootstrapper
    {
        public static void ResetContainer()
        {
            ObjectFactory.ResetDefaults();
            BootstrapStructureMap();
        }


        public static void BootstrapStructureMap()
        {
            // TODO: test
            Startup.Init<Product>("http://localhost:8983/solr/product");
            Startup.Init<Manufacturer>("http://localhost:8983/solr/manufacturer");

            var solrConfig = (SolrConfigurationSection)ConfigurationManager.GetSection("solr");
            ObjectFactory.Initialize(x =>
            {
                x.Scan(s =>
                {
                    s.TheCallingAssembly();
                    s.AssemblyContainingType<IUnitOfWork>();
                    s.Assembly(typeof(SolrNetRegistry).Assembly);
                    s.Assembly(typeof(SolrConnection).Assembly);
                    s.Assembly(typeof(SolrSession).Assembly);
                    s.Assembly(typeof(MemoizingMappingManager).Assembly);

                    s.LookForRegistries();
                });

                x.AddRegistry(new SolrNetRegistry(solrConfig.SolrServers));

            });
        }
    }
}