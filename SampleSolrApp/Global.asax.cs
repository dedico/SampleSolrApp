#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using log4net.Config;
using Microsoft.Practices.ServiceLocation;

using SampleSolrApp.Models;
using SampleSolrApp.Models.Nh;
using SampleSolrApp.Models.Binders;
using SampleSolrApp.Core.Ioc;
using SampleSolrApp.Core.Repository;

using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.Exceptions;
using SolrNet.Impl;

using NHibernate.Tool.hbm2ddl;
using StructureMap;

namespace SampleSolrApp
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}", // URL with parameters
                new { controller = "Home", action = "Index" } // Parameter defaults
                );
        }

        protected void Application_Start()
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(Server.MapPath("/"), "log4net.config")));

            RegisterRoutes(RouteTable.Routes);

            ContainerBootstrapper.BootstrapStructureMap();
            ControllerBuilder.Current.SetControllerFactory(new StructureMapControllerFactory());

            ClearDatabaseAndIndex();

            ModelBinders.Binders[typeof(SearchParameters)] = new SearchParametersBinder();
        }


        private void ClearDatabaseAndIndex()
        {

            new SchemaExport(ObjectFactory.GetInstance<NHibernate.Cfg.Configuration>()).Drop(true, true);
            new SchemaExport(ObjectFactory.GetInstance<NHibernate.Cfg.Configuration>()).Create(true, true);

            var solr = ObjectFactory.GetInstance<ISolrOperations<Product>>();
            solr.Delete(SolrQuery.All);
            solr.Commit();
            solr.BuildSpellCheckDictionary();

            var solr1 = ObjectFactory.GetInstance<ISolrOperations<Manufacturer>>();
            solr1.Delete(SolrQuery.All);
            solr1.Commit();
            solr1.BuildSpellCheckDictionary();
        }
    }
}