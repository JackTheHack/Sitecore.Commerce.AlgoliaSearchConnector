using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Plugin.Commerce.Search.Algolia.Models;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Search;
using Sitecore.Framework.Pipelines;

namespace Plugin.Commerce.Search.Algolia.Blocks
{
    public class FullIndexStartedBlock : PipelineBlock<SearchIndexArgument, SearchIndexArgument, CommercePipelineExecutionContext>
    {
        private readonly bool _enableIndexing;

        public FullIndexStartedBlock(IConfiguration configuration)
        {
            _enableIndexing = configuration.GetValue("AppSettings:EnableIndexing", true);
        }

        public override Task<SearchIndexArgument> Run(SearchIndexArgument arg, CommercePipelineExecutionContext context)
        {
            if (!_enableIndexing)
            {
                context.Logger.LogWarning($"[{arg.IndexName}] Indexing Disabled. Skipping");
                context.Abort("Indexing disabled.", context);
            }

            FullIndexUpdateStatus.ItemsProcessed = 0;
            context.Logger.LogInformation($"[{arg.IndexName}] Full indexing started.");
            return Task.FromResult(arg);
        }
    }
}
