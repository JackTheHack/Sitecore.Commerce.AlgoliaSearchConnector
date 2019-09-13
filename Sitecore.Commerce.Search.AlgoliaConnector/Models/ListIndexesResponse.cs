using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Commerce.Search.Algolia.Models
{
    public class ListIndexesResponse
    {
        public List<AlgoliaIndex> items { get; set; }
        public int nbPages { get; set; }
    }
}
