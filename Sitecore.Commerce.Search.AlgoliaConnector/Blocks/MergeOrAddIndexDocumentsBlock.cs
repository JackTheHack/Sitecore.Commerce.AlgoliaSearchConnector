using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Search;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Plugin.Commerce.Search.Algolia.Blocks
{
    public class MergeOrAddIndexDocumentsBlock : ConditionalPipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly AlgoliaContextCommand _command;

        public MergeOrAddIndexDocumentsBlock(AlgoliaContextCommand command)
        {
            this._command = command;
            this.BlockCondition = PipelineBlockHelper.ValidatePolicy;
        }

        public override async Task<EntityView> Run(EntityView arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{Name}: view can not be null.");

            IEnumerable<JObject> source = 
                context.CommerceContext.GetObjects<IEnumerable<JObject>>().FirstOrDefault();

            if (source == null || arg == null)
            {
                return arg;
            }

            JObject[] documentArray = source as JObject[] ?? source.ToArray();

            Condition.Requires(documentArray).IsNotNull($"{Name}: documents can not be null.");

            if (!documentArray.Any() || !arg.HasPolicy<SearchScopePolicy>())
            {
                return arg;
            }

            string name = arg.GetPolicy<SearchScopePolicy>()?.Name;
            if (string.IsNullOrEmpty(name))
            {
                return arg;
            }

            IEnumerable<IndexingResult> indexingResults = 
                await _command.MergeOrAddDocuments(name, documentArray, context.CommerceContext);

            if (indexingResults != null)
            {
                context.CommerceContext.AddObject(indexingResults);
            }

            return arg;
        }

        public override Task<EntityView> ContinueTask(EntityView arg, CommercePipelineExecutionContext context)
        {
            return Task.FromResult(arg);
        }
    }
}