using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Commerce.Search.Algolia.Models;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Search;
using Sitecore.Framework.Conditions;

namespace Plugin.Commerce.Search.Algolia.Blocks
{
    public class ListIndexesBlock : ConditionalPipelineBlock<SearchArgument, SearchArgument, CommercePipelineExecutionContext>
    {
        private readonly AlgoliaContextCommand _command;

        public ListIndexesBlock(AlgoliaContextCommand command)
        {
            _command = command;
            BlockCondition = PipelineBlockHelper.ValidatePolicy;
        }

        public override async Task<SearchArgument> Run(SearchArgument arg, CommercePipelineExecutionContext context)
        {
            // ISSUE: explicit non-virtual call
            Condition.Requires(arg).IsNotNull(
                $"{nameof(Name)}: argument cannot be null.");

            var listIndexesResponse = await _command.ListSearchIndexesAsync(context.CommerceContext);

            EntityView entityView = new EntityView
            {
                Name = context.GetPolicy<KnownSearchViewsPolicy>().Indexes
            };
            EntityView view = entityView;
            context.CommerceContext.AddObject(view);

            if (listIndexesResponse?.items == null || !listIndexesResponse.items.Any())
                return arg;

            listIndexesResponse.items.ForEach(index =>
            {
                EntityView indexView = this.CreateIndexView(index, context);
                if (indexView == null)
                    return;
                view.ChildViews.Add(indexView);
            });

            return arg;
        }

        private EntityView CreateIndexView(AlgoliaIndex index, CommercePipelineExecutionContext context)
        {
            EntityView view = new EntityView();
            if (index == null)
                return view;
            view.Name = context.GetPolicy<KnownSearchViewsPolicy>().Index;
            view.EntityId = index.name;
            List<ViewProperty> properties1 = view.Properties;
            ViewProperty viewProperty = new ViewProperty
            {
                Name = "Name",
                RawValue = (object) index.name,
                IsReadOnly = true
            };
            properties1.Add(viewProperty);            
            return view;
        }

        public override Task<SearchArgument> ContinueTask(SearchArgument arg, CommercePipelineExecutionContext context)
        {
            return Task.FromResult(arg);
        }
    }
}