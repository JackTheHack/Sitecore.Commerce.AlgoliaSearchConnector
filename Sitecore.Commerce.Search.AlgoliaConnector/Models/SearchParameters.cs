using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Commerce.Search.Algolia.Models
{
    public class SearchParameters
    {
        public IList<string> Select { get; set; }
        public string Filter { get; set; }
        public IList<string> OrderBy { get; set; }
        public int? Skip { get; set; }
        public int? Top { get; set; }
    }
}
