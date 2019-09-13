using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plugin.Commerce.Search.Algolia.Models;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Search;
using Sitecore.Framework.Pipelines;

namespace Plugin.Commerce.Search.Algolia.Blocks
{
    public class FullIndexCompletedBlock : PipelineBlock<IEnumerable<IndexingResult>, IEnumerable<IndexingResult>, CommercePipelineExecutionContext>
    {
        public override Task<IEnumerable<IndexingResult>> Run(IEnumerable<IndexingResult> arg, CommercePipelineExecutionContext context)
        {
            var indexingResults = arg.ToList();

            FullIndexUpdateStatus.ItemsProcessed += indexingResults.Count;

            context.Logger.LogInformation($"Full index - {FullIndexUpdateStatus.ItemsProcessed} items processed.");
            return Task.FromResult(indexingResults.AsEnumerable());
        }
    }
}
