using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using StructureMap;
using System.Reflection;


namespace SampleSolrApp.Core.Repository
{
    public class Repository : INhRepository
    {
        private ISession _session;

        public ISession CurrentSession { get { return _session; } }


        public string CurrentLanguage
        {
            get { return Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant(); }
        }

        public Repository(ISession session)
        {
            _session = session;
        }

        public void OverrideSession(ISession session)
        {
            _session = session;
        }

        public void Save(object entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        public void Merge(object entity)
        {
            CurrentSession.Merge(entity);
        }

        public void Delete(object entity)
        {
            CurrentSession.Delete(entity);
        }

        public void Flush()
        {
            CurrentSession.Flush();
        }

        public void ClearSession()
        {
            CurrentSession.Clear();
        }

        public IEnumerable<T> FindAll<T>()
        {
            return _session.CreateCriteria(typeof(T)).List<T>().AsEnumerable();
        }


        public T Find<T>(int id)
        {
            ICriterion criterion = Restrictions.Eq("Id", id);
            return _session.CreateCriteria(typeof(T)).Add(criterion).UniqueResult<T>();
        }

        public T Find<T>(string id)
        {
            ICriterion criterion = Restrictions.Eq("Id", id);
            return _session.CreateCriteria(typeof(T)).Add(criterion).UniqueResult<T>();
        }

        public T Find<T>(Guid id)
        {
            ICriterion criterion = Restrictions.Eq("Id", id);
            return _session.CreateCriteria(typeof(T)).Add(criterion).UniqueResult<T>();
        }

        public T FindRecord<T>(int id)
        {
            return _session.CreateCriteria(typeof(T))
                .Add(Restrictions.Eq("Id", id))
                .CreateCriteria("Translations", JoinType.LeftOuterJoin)
                .SetResultTransformer(new DistinctRootEntityResultTransformer())
                .UniqueResult<T>();
        }

        public T FindByName<T>(string name)
        {
            ICriterion criterion = Restrictions.Eq("Name", name).IgnoreCase();

            return _session.CreateCriteria(typeof(T)).Add(criterion).UniqueResult<T>();
        }



        public IEnumerable<T> Find<T>(ICollection<ICriterion> criterions)
        {
            var criteria = _session.CreateCriteria(typeof(T));

            foreach (var criterion in criterions)
            {
                criteria.Add(criterion);
            }
            return criteria.List<T>().AsEnumerable();
        }

        public IEnumerable<T> Find<T>(ICriterion criterion)
        {
            return _session.CreateCriteria(typeof(T)).Add(criterion).List<T>().AsEnumerable();
        }

        public IEnumerable<T> FindByHql<T>(string hql, string[] paramName, object[] paramValue)
        {
            var query = _session.CreateQuery(hql);
            foreach (var p in paramName)
            {
                query = query.SetParameter(p, paramValue[Array.IndexOf(paramName, p)]);
            }

            return query.List<T>().AsEnumerable();
        }

    }
}