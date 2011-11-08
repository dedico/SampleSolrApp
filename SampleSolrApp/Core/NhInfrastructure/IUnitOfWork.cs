using System;
using NHibernate;

namespace SampleSolrApp.Core.NhInfrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        ISession CurrentSession { get; }
        void Commit();
        void Rollback();
    }
}