using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SampleSolrApp.Models;

using NHibernate;
using NHibernate.Criterion;

namespace SampleSolrApp.Core.Repository
{
    public class OrderRepository : NhBaseRepository<SampleSolrApp.Models.Nh.Order, int> 
    {

        public OrderRepository(ISession session) : base(session) { }

        public override SampleSolrApp.Models.Nh.Order Find(int id)
        {
            return CurrentSession.QueryOver<SampleSolrApp.Models.Nh.Order>()
                    .Where(x => x.Id == id)
                    .SingleOrDefault();
        }
    }
}