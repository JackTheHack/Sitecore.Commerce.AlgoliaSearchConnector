using System.Collections.Generic;
using System.Threading.Tasks;
using Algolia.Search;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Search;
using Sitecore.Framework.Conditions;

namespace Plugin.Commerce.Search.Algolia.Blocks
{
    public class GetIndexBlock : ConditionalPipelineBlock<SearchIndexArgument, SearchIndexArgument, CommercePipelineExecutionContext>
    {
        private readonly AlgoliaContextCommand _command;

        public GetIndexBlock(AlgoliaContextCommand command)
        {
            _command = command;
            BlockCondition = PipelineBlockHelper.ValidatePolicy;
        }

        public override async Task<SearchIndexArgument> Run(SearchIndexArgument arg, CommercePipelineExecutionContext context)
        {
            // ISSUE: explicit non-virtual call
            Condition.Requires(arg).IsNotNull($"{nameof(Name)}: argument cannot be null.");
            Index searchIndex = _command.GetSearchIndex(arg.IndexName, context.CommerceContext);
            context.CommerceContext.AddObject(CreateIndexView(searchIndex, arg.IndexName, context));

            if (searchIndex != null)
            {
                return arg;
            }

            CommercePipelineExecutionContext executionContext = context;
            CommerceContext commerceContext = context.CommerceContext;

            string error = context.GetPolicy<KnownResultCodes>().Error;
            string commerceTermKey = "EntityNotFound";
            string defaultMessage = $"Entity '{arg.IndexName}' was not found.";
            executionContext.Abort(await commerceContext.AddMessage(error, commerceTermKey, new object[]{arg.IndexName}, defaultMessage), context);
            return arg;
        }

        protected virtual EntityView CreateIndexView(Index index, string indexName, CommercePipelineExecutionContext context)
        {
            EntityView entityView1 = new EntityView {Name = context.GetPolicy<KnownSearchViewsPolicy>().Index};
            EntityView view = entityView1;
            if (index == null)
                return view;
            view.EntityId = indexName;
            List<ViewProperty> properties1 = view.Properties;
            ViewProperty viewProperty = new ViewProperty
            {
                Name = "Name",
                RawValue = indexName,
                IsReadOnly = true
            };
            properties1.Add(viewProperty);
            return view;
        }

        public override Task<SearchIndexArgument> ContinueTask(SearchIndexArgument arg, CommercePipelineExecutionContext context)
        {
            return Task.FromResult(arg);
        }
    }
}