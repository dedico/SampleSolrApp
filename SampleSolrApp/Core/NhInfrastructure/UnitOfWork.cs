using System;
using System.Threading;
using NHibernate;

namespace SampleSolrApp.Core.NhInfrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ISessionFactory _sessionFactory;
        private readonly ITransaction _transaction;

        public UnitOfWork(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
            CurrentSession = _sessionFactory.OpenSession();
            //CurrentSession.EnableFilter("translationFilter").SetParameter("locale", Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName);
            _transaction = CurrentSession.BeginTransaction();
        }

        public ISession CurrentSession { get; private set; }

        public void Dispose()
        {
            if (CurrentSession == null) return;
            CurrentSession.Close();
            CurrentSession = null;
        }

        public void Commit()
        {
            try
            {
                if (_transaction.IsActive)
                {
                    System.Diagnostics.Debug.WriteLine("*** Before Transaction.Commit() ***");
                    _transaction.Commit();
                    System.Diagnostics.Debug.WriteLine("*** After Transaction.Commit() ***");
                }
            }
            catch (Exception)
            {
                Rollback();
                throw;
            }
        }

        public void Rollback()
        {
            if (_transaction.IsActive)
                _transaction.Rollback();
            if (CurrentSession == null) return;
            CurrentSession.Close();
            CurrentSession = null;
        }
    }
}