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
using SampleSolrApp.Models.Nh;
using SampleSolrApp.Core.NhInfrastructure;
using SampleSolrApp.Core.Repository;

using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.DSL;
using SolrNet.Exceptions;

using StructureMap;

namespace SampleSolrApp.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        private readonly ISolrReadOnlyOperations<Product> solr;
        private readonly ISolrOperations<Product> writer;
        private readonly IList<string> categories;
        private readonly IList<string> manufactures;


        public HomeController(ISolrReadOnlyOperations<Product> solr, ISolrOperations<Product> writer)
        {
            this.solr = solr;
            this.writer = writer;
            this.categories = new List<string> { "myszki", "klawiatury", "mnitory", "rtv", "agd", "cpu", "dyski" };
            this.manufactures = new List<string> { "seagate", "samsung", "dell", "hp", "apple", "microsoft", "logitech" };

        }

        /// <summary>
        /// Builds the Solr query from the search parameters
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ISolrQuery BuildQuery(SearchParameters parameters)
        {
            if (!string.IsNullOrEmpty(parameters.FreeSearch))
                return new SolrQuery(parameters.FreeSearch);
            return SolrQuery.All;
        }

        public ICollection<ISolrQuery> BuildFilterQueries(SearchParameters parameters)
        {
            var queriesFromFacets = from p in parameters.Facets
                                    select (ISolrQuery)Query.Field(p.Key).Is(p.Value);
            return queriesFromFacets.ToList();
        }


        /// <summary>
        /// All selectable facet fields
        /// </summary>
        private static readonly string[] AllFacetFields = new[] { "cat", "manu_exact" };

        /// <summary>
        /// Gets the selected facet fields
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IEnumerable<string> SelectedFacetFields(SearchParameters parameters)
        {
            return parameters.Facets.Select(f => f.Key);
        }

        public SortOrder[] GetSelectedSort(SearchParameters parameters)
        {
            return new[] { SortOrder.Parse(parameters.Sort) }.Where(o => o != null).ToArray();
        }

        public ActionResult Index(SearchParameters parameters)
        {

            try
            {
                var start = (parameters.PageIndex - 1) * parameters.PageSize;
                var matchingProducts = solr.Query(BuildQuery(parameters), new QueryOptions
                {
                    FilterQueries = BuildFilterQueries(parameters),
                    Rows = parameters.PageSize,
                    Start = start,
                    OrderBy = GetSelectedSort(parameters),
                    SpellCheck = new SpellCheckingParameters(),
                    Facet = new FacetParameters
                    {
                        Queries = AllFacetFields.Except(SelectedFacetFields(parameters))
                                                                              .Select(f => new SolrFacetFieldQuery(f) { MinCount = 1 })
                                                                              .Cast<ISolrFacetQuery>()
                                                                              .ToList(),
                    },
                });
                var view = new ProductView<Product>
                {
                    Results = matchingProducts,
                    Search = parameters,
                    TotalCount = matchingProducts.NumFound,
                    Facets = matchingProducts.FacetFields,
                    DidYouMean = GetSpellCheckingResult(matchingProducts),
                };
                return View(view);
            }
            catch (InvalidFieldException)
            {
                return View(new ProductView<Product>
                {
                    QueryError = true,
                });
            }
        }

        public ActionResult Delete(int id)
        {
            var r = ObjectFactory.GetInstance<ProductRepository>();
            var product = r.Find(id);
            r.Delete(product);
            return RedirectToAction("Index");
        }


        public ActionResult Add()
        {

            var r = ObjectFactory.GetInstance<ProductRepository>();

            var p = new Product();
            var rnd = new Random();

            var c = categories[rnd.Next(categories.Count - 1)];
            var m = manufactures[rnd.Next(manufactures.Count - 1)]; ;

            p.Categories.Add(c);
            p.Features.Add(m);
            p.Features.Add(c);
            p.Features.Add("termostat");
            //p.Id = Guid.NewGuid().ToString();
            p.InStock = true;
            p.Manufacturer = m;
            p.Name = m + " " + c;
            p.Popularity = rnd.Next(100);
            p.Price = (decimal)(rnd.NextDouble() * rnd.Next(100) + rnd.NextDouble());
            p.SKU = "SKU";
            p.Timestamp = DateTime.Now;
            p.Weight = (rnd.NextDouble() * rnd.Next(100) + rnd.NextDouble());
            p.Description = "producent " + m + " kategoria " + c;

            r.Save(p);


            //writer.Add(p);
            //writer.Commit();

            return RedirectToAction("Index");
        }

        public ActionResult Reindex()
        {

            var r = ObjectFactory.GetInstance<ProductRepository>();

            foreach (var p in r.FindAll())
                writer.Add(p);

            writer.Commit();
            return RedirectToAction("Index");
        }

        private string GetSpellCheckingResult(ISolrQueryResults<Product> products)
        {
            return string.Join(" ", products.SpellChecking
                                        .Select(c => c.Suggestions.FirstOrDefault())
                                        .Where(c => !string.IsNullOrEmpty(c))
                                        .ToArray());
        }
    }
}