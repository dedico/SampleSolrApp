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

namespace SampleSolrApp.Controllers {
    [HandleError]
    public class ManufacturerController : Controller
    {
        private readonly ISolrReadOnlyOperations<Manufacturer> solr;
        private readonly ISolrOperations<Manufacturer> writer;
        private readonly IList<string> names;



        public ManufacturerController(ISolrReadOnlyOperations<Manufacturer> solr, ISolrOperations<Manufacturer> writer)
        {
            this.solr = solr;
            this.writer = writer;
            this.names = new List<string> { "samsung", "microsoft", "dell", "logitech", "hp", "panasonic", "sony", "assus", "gigabyte", "amt", "nvidia", "toshiba", "lexmark" };

        }

        /// <summary>
        /// Builds the Solr query from the search parameters
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ISolrQuery BuildQuery(SearchParameters parameters) {
            if (!string.IsNullOrEmpty(parameters.FreeSearch))
                return new SolrQuery(parameters.FreeSearch);
            return SolrQuery.All;
        }

        public ICollection<ISolrQuery> BuildFilterQueries(SearchParameters parameters) {
            var queriesFromFacets = from p in parameters.Facets
                                    select (ISolrQuery)Query.Field(p.Key).Is(p.Value);
            return queriesFromFacets.ToList();
        }


        /// <summary>
        /// All selectable facet fields
        /// </summary>
        private static readonly string[] AllFacetFields = new[] {"cat", "manu_exact"};

        /// <summary>
        /// Gets the selected facet fields
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IEnumerable<string> SelectedFacetFields(SearchParameters parameters) {
            return parameters.Facets.Select(f => f.Key);
        }

        public SortOrder[] GetSelectedSort(SearchParameters parameters) {
            return new[] {SortOrder.Parse(parameters.Sort)}.Where(o => o != null).ToArray();
        }

        public ActionResult Index(SearchParameters parameters) {
            try {
                var start = (parameters.PageIndex - 1)*parameters.PageSize;

                var query = BuildQuery(parameters);

                var matchingProducts = solr.Query(query, new QueryOptions {
                    FilterQueries = BuildFilterQueries(parameters),
                    Rows = parameters.PageSize,
                    Start = start,
                    OrderBy = GetSelectedSort(parameters),
                    SpellCheck = new SpellCheckingParameters(),
                    Facet = new FacetParameters {
                        Queries = AllFacetFields.Except(SelectedFacetFields(parameters))
                                                                              .Select(f => new SolrFacetFieldQuery(f) {MinCount = 1})
                                                                              .Cast<ISolrFacetQuery>()
                                                                              .ToList(),
                    },
                });
                var view = new ProductView<Manufacturer> {
                    Results = matchingProducts,
                    Search = parameters,
                    TotalCount = matchingProducts.NumFound,
                    Facets = matchingProducts.FacetFields,
                    DidYouMean = GetSpellCheckingResult(matchingProducts),
                };
                return View(view);
            } catch (InvalidFieldException) {
                return View(new ProductView<Manufacturer> {
                    QueryError = true,
                });
            }
        }

        public ActionResult Add()
        {

            var r = ObjectFactory.GetInstance<ProductRepository>();

            var p = new Manufacturer();
            var rnd = new Random();

            var name = names[rnd.Next(names.Count - 1)];

            p.Name = name;
            r.Save(p);

            //writer.Add(p);
            //writer.Commit();

            return RedirectToAction("Index");
        }

        public ActionResult Reindex()
        {
            var r = ObjectFactory.GetInstance<ProductRepository>();

            foreach (var m in r.CurrentSession.QueryOver<Manufacturer>().List())
                writer.Add(m);

            writer.Commit();
            return RedirectToAction("Index");
        }


        private string GetSpellCheckingResult(ISolrQueryResults<Manufacturer> products) {
            return string.Join(" ", products.SpellChecking
                                        .Select(c => c.Suggestions.FirstOrDefault())
                                        .Where(c => !string.IsNullOrEmpty(c))
                                        .ToArray());
        }
    }
}