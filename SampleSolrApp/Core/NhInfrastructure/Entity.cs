using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleSolrApp.Core.NhInfrastructure
{
    /// <summary>
    /// Podstawowy interfejs obiektu zapisywalnego w bazie danych
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEntity<T>
    {
        T Id { get; }
    }
    [Serializable]
    public abstract class Entity<T> : IEntity<T>
    {
        public virtual T Id { get; set; }

    }
}