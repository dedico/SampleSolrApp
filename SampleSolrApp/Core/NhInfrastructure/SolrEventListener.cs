using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate.Event;

namespace SampleSolrApp.Core.NhInfrastructure
{
    public class SolrEventListener : IPostInsertEventListener, IPostUpdateEventListener, IPostDeleteEventListener
    {
        public void OnPostInsert(PostInsertEvent @event)
        {
            System.Diagnostics.Debug.WriteLine("*** OnPostInsert Start ***");

            var product = @event.Entity as SampleSolrApp.Models.Nh.Product;
            if (product != null)
            {
                var productWriter = StructureMap.ObjectFactory.GetInstance<SolrNet.ISolrOperations<SampleSolrApp.Models.Nh.Product>>();
                productWriter.Add(product);
                productWriter.Commit();
                System.Diagnostics.Debug.WriteLine("*** Product inserted and commited ***");
            }

            var manufacturer = @event.Entity as SampleSolrApp.Models.Nh.Manufacturer;
            if (manufacturer != null)
            {
                var manufacturerWriter = StructureMap.ObjectFactory.GetInstance<SolrNet.ISolrOperations<SampleSolrApp.Models.Nh.Manufacturer>>();
                manufacturerWriter.Add(manufacturer);
                manufacturerWriter.Commit();
                System.Diagnostics.Debug.WriteLine("*** Manufacturer inserted and commited ***");
            }

            System.Diagnostics.Debug.WriteLine("*** OnPostInsert End ***");
        }

        public void OnPostUpdate(PostUpdateEvent @event)
        {
            System.Diagnostics.Debug.WriteLine("*** OnPostUpdate Start ***");

            var product = @event.Entity as SampleSolrApp.Models.Nh.Product;
            if (product != null)
            {
                var productWriter = StructureMap.ObjectFactory.GetInstance<SolrNet.ISolrOperations<SampleSolrApp.Models.Nh.Product>>();
                productWriter.Add(product);
                productWriter.Commit();
                System.Diagnostics.Debug.WriteLine("*** Product updated and commited ***");
            }

            var manufacturer = @event.Entity as SampleSolrApp.Models.Nh.Manufacturer;
            if (manufacturer != null)
            {
                var manufacturerWriter = StructureMap.ObjectFactory.GetInstance<SolrNet.ISolrOperations<SampleSolrApp.Models.Nh.Manufacturer>>();
                manufacturerWriter.Add(manufacturer);
                manufacturerWriter.Commit();
                System.Diagnostics.Debug.WriteLine("*** Manufacturer updated and commited ***");
            }

            System.Diagnostics.Debug.WriteLine("*** OnPostUpdate End ***");
        }

        public void OnPostDelete(PostDeleteEvent @event)
        {
            System.Diagnostics.Debug.WriteLine("*** OnPostDelete Start ***");

            var product = @event.Entity as SampleSolrApp.Models.Nh.Product;
            if (product != null)
            {
                var productWriter = StructureMap.ObjectFactory.GetInstance<SolrNet.ISolrOperations<SampleSolrApp.Models.Nh.Product>>();
                productWriter.Delete(product);
                productWriter.Commit();
                System.Diagnostics.Debug.WriteLine("*** Product deleted and commited ***");
            }

            var manufacturer = @event.Entity as SampleSolrApp.Models.Nh.Manufacturer;
            if (manufacturer != null)
            {
                var manufacturerWriter = StructureMap.ObjectFactory.GetInstance<SolrNet.ISolrOperations<SampleSolrApp.Models.Nh.Manufacturer>>();
                manufacturerWriter.Delete(manufacturer);
                manufacturerWriter.Commit();
                System.Diagnostics.Debug.WriteLine("*** Manufacturer deleted and commited ***");
            }

            System.Diagnostics.Debug.WriteLine("*** OnPostDelete End ***");
        }
    }
}