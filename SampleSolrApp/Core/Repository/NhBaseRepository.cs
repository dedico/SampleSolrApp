using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using SampleSolrApp.Core.NhInfrastructure;

using NHibernate;
using NHibernate.Criterion;


namespace SampleSolrApp.Core.Repository
{
    public abstract class NhBaseRepository<T, TK> : INhRepository<T, TK> where T : Entity<TK>
    {
        private ISession _session;

        protected NhBaseRepository(ISession session)
        {
            _session = session;
        }

        public ISession CurrentSession
        {
            get { return _session; }
        }

        public string CurrentLanguage
        {
            get { return Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant(); }
        }

        public void OverrideSession(ISession session)
        {
            _session = session;
        }

        public virtual T Find(TK id)
        {
            return _session.QueryOver<T>()
                           .Where(Restrictions.Eq("Id", id))
                //.Where(x=>x.Id == id) //nie wiem dlaczego to nie chce zadzialac...
                           .SingleOrDefault();
        }

        public IEnumerable<T> FindAll()
        {
            return _session.QueryOver<T>()
                           .List();
        }

        public void Save(object entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        public void Delete(object entity)
        {
            CurrentSession.Delete(entity);
        }

        public void Flush()
        {
            CurrentSession.Flush();
        }

        public void Merge(object entity)
        {
            CurrentSession.Merge(entity);
        }

        public void ClearSession()
        {
            CurrentSession.Clear();
        }

    }
}
