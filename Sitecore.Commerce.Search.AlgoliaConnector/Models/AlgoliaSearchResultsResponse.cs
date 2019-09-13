using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Plugin.Commerce.Search.Algolia.Models
{
    public class AlgoliaSearchResultsResponse
    {
        public List<JObject> hits { get; set; }
        public int page { get; set; }
        public int nbHits { get; set; }
        public int nbPages { get; set; }
        public int hitsPerPage { get; set; }
        public int processingTimeMS { get; set; }
        public string query { get; set; }
        public string @params {get;set;}
    }
}
