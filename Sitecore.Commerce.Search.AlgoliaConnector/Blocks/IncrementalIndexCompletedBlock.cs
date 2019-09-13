using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Search;
using Sitecore.Framework.Pipelines;

namespace Plugin.Commerce.Search.Algolia.Blocks
{
    public class IncrementalIndexCompletedBlock : PipelineBlock<IEnumerable<IndexingResult>, IEnumerable<IndexingResult>, CommercePipelineExecutionContext>
    {
        public override Task<IEnumerable<IndexingResult>> Run(IEnumerable<IndexingResult> arg, CommercePipelineExecutionContext context)
        {
            var indexingResults = arg.ToList();
            context.Status.Report($"Incremental indexing complete - {indexingResults.Count} items.");
            context.Logger.LogInformation($"Incremental indexing complete - {indexingResults.Count} items.");
            return Task.FromResult(indexingResults.AsEnumerable());
        }
    }
}
