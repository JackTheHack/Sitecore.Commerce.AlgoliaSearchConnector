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
    public class DeleteIndexDocumentsBlock : ConditionalPipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly AlgoliaContextCommand _command;

        public DeleteIndexDocumentsBlock(AlgoliaContextCommand command)
        {
            this._command = command;
            this.BlockCondition = PipelineBlockHelper.ValidatePolicy;
        }

        public override async Task<EntityView> Run(EntityView arg, CommercePipelineExecutionContext context)
        {
            IEnumerable<JObject> source = context.CommerceContext.GetObjects<IEnumerable<JObject>>().FirstOrDefault();

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

            string name = arg.GetPolicy<SearchScopePolicy>().Name;
            if (string.IsNullOrEmpty(name))
            {
                return arg;
            }

            var ids = documentArray.Select(i => i.Property("id").Value<string>()).ToList();

            context.CommerceContext.AddObject(await _command.DeleteDocuments(name, ids, context.CommerceContext));
            return arg;
        }

        public override Task<EntityView> ContinueTask(EntityView arg, CommercePipelineExecutionContext context)
        {
            return Task.FromResult(arg);
        }
    }
}