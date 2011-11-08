using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using StructureMap;

namespace SampleSolrApp.Core.Ioc
{
    public class StructureMapServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            return ObjectFactory.GetInstance(serviceType);
        }

        public T GetService<T>()
        {
            return ObjectFactory.GetInstance<T>();
        }
    }
}