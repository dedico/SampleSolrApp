#region license
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
using System.Web.Mvc;

using SampleSolrApp.Models;
using SampleSolrApp.Core.NhInfrastructure;
using SampleSolrApp.Core.Repository;

using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.DSL;
using SolrNet.Exceptions;

using StructureMap;

namespace SampleSolrApp.Controllers {
    [HandleError]
    public class OrderController : Controller
    {
        private readonly IList<string> names;
        private readonly OrderRepository orderRepository;

        public OrderController(OrderRepository orderRepository)
        {
            this.orderRepository = orderRepository;
            this.names = new List<string> { "samsung", "microsoft", "dell", "logitech", "hp", "panasonic", "sony", "assus", "gigabyte", "amt", "nvidia", "toshiba", "lexmark" };
        }



        public ActionResult Index() {

            var orders = orderRepository.FindAll();
            return View(orders);

        }

        public ActionResult Add()
        {

            var p = new  SampleSolrApp.Models.Nh.Order();
            var rnd = new Random();

            var name = names[rnd.Next(names.Count - 1)];

            p.Name = name;
            p.Amount = (decimal)(rnd.NextDouble() * 100 + rnd.NextDouble());
            orderRepository.Save(p);


            return RedirectToAction("Index");
        }
    }
}