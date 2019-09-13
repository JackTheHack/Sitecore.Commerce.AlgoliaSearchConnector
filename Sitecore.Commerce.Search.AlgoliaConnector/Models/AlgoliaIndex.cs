using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Commerce.Search.Algolia.Models
{
    public class AlgoliaIndex
    {
        public string name { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public int entries { get; set; }
        public int dataSize { get; set; }
        public int fileSize { get; set; }
        public int lastBuildTimeS { get; set; }
        public int numberOfPendingTasks { get; set; }
        public bool pendingTask { get; set; }
    }
}
