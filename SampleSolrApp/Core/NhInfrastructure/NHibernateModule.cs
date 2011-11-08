using System;
using System.Web;
using StructureMap;

namespace SampleSolrApp.Core.NhInfrastructure
{
    public class NHibernateModule : IHttpModule, IDisposable
    {
        private IUnitOfWork _unitOfWork;


        public void Init(HttpApplication context)
        {
            context.BeginRequest += ContextBeginRequest;
            context.EndRequest += ContextEndRequest;
        }

        private void ContextBeginRequest(object sender, EventArgs e)
        {
            _unitOfWork = ObjectFactory.GetInstance<IUnitOfWork>();

        }

        private void ContextEndRequest(object sender, EventArgs e)
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_unitOfWork != null)
            {
                _unitOfWork.Commit();
                _unitOfWork.Dispose();
            }
        }
    }
}