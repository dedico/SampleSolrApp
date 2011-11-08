using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SampleSolrApp.Models.Nh;

using NHibernate;
using NHibernate.Criterion;

namespace SampleSolrApp.Core.Repository
{
    public class ProductRepository : NhBaseRepository<Product, int> 
    {
    
        public ProductRepository(ISession session) : base(session) { }

        public override Product Find(int id)
        {
            return CurrentSession.QueryOver<Product>()
                    .Where(x => x.Id == id)
                    .SingleOrDefault();
        }
    }
}