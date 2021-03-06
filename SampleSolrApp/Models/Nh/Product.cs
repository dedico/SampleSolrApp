﻿#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;

using SolrNet.Attributes;
using SampleSolrApp.Core.NhInfrastructure;

using Iesi.Collections.Generic;

namespace SampleSolrApp.Models.Nh {
    public class Product : Entity<int>
    {
        [SolrUniqueKey("id")]
        public virtual int Id { get; set; }

        [SolrField("sku")]
        public virtual string SKU { get; set; }

        [SolrField("name")]
        public virtual string Name { get; set; }

        [SolrField("manu_exact")]
        public virtual string Manufacturer { get; set; }

        [SolrField("description")]
        public virtual string Description { get; set; }

        
        public virtual ISet<string> Categories { get; set; }

        [SolrField("cat")]
        public virtual ICollection<string> Cat { get { return Categories.ToList(); } }

        
        public virtual ISet<string> Features { get; set; }

        [SolrField("features")]
        public virtual ICollection<string> Feat { get { return Features.ToList(); } }

        [SolrField("price")]
        public virtual decimal Price { get; set; }

        [SolrField("popularity")]
        public virtual int Popularity { get; set; }

        [SolrField("inStock")]
        public virtual bool InStock { get; set; }

        [SolrField("timestamp")]
        public virtual DateTime? Timestamp { get; set; }

        [SolrField("weight")]
        public virtual double? Weight { get; set; }

        public Product()
        {
           Categories = new HashedSet<string>();
           Features = new HashedSet<string>();
        }

    }
}