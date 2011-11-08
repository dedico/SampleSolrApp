using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using NHibernate;
using NHibernate.Criterion;

namespace SampleSolrApp.Core.Repository
{
    public interface INhRepository<T, TK>
    {
        ISession CurrentSession { get; }
        string CurrentLanguage { get; }
        void OverrideSession(ISession session);
        void Save(object entity);
        void Delete(object entity);
        void Flush();
        void Merge(object entity);
        void ClearSession();
        T Find(TK id);
        IEnumerable<T> FindAll();
    }

    public interface INhRepository
    {
        ISession CurrentSession { get; }
        string CurrentLanguage { get; }
        void OverrideSession(ISession session);
        void Save(object entity);
        void Delete(object entity);
        void Flush();
        void Merge(object entity);
        void ClearSession();

        IEnumerable<T> FindAll<T>();
        T Find<T>(int id);
        T Find<T>(string code);
        T Find<T>(Guid id);
        T FindRecord<T>(int id);
        T FindByName<T>(string name);
        IEnumerable<T> Find<T>(ICollection<ICriterion> criterions);
        IEnumerable<T> Find<T>(ICriterion criterion);
    }
}