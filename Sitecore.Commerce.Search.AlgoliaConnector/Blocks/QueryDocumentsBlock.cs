using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Commerce.Search.Algolia.Models;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Search;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Plugin.Commerce.Search.Algolia.Blocks
{
    public class QueryDocumentsBlock : ConditionalPipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private AlgoliaContextCommand _searchCommand;

        public QueryDocumentsBlock(AlgoliaContextCommand azureContextCommand)
        {
            this._searchCommand = azureContextCommand;
            this.BlockCondition = PipelineBlockHelper.ValidatePolicy;
        }


        public override async Task<EntityView> Run(EntityView arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{Name}: argument can not be null.");

            CommercePipelineExecutionContext executionContext;

            if (!arg.HasPolicy<SearchScopePolicy>())
            {
                executionContext = context;
                executionContext.Abort(
                    await context.CommerceContext.AddMessage(context.GetPolicy<KnownResultCodes>().Error,
                        "DocumentsViewError", null,
                        "Can not perform the query. Documents view is missing information."), context);
                return null;
            }

            SearchScopePolicy scopePolicy = arg.GetPolicy<SearchScopePolicy>();
            if (string.IsNullOrEmpty(scopePolicy.Name))
            {
                executionContext = context;
                executionContext.Abort(
                    await context.CommerceContext.AddMessage(context.GetPolicy<KnownResultCodes>().Error,
                        "DocumentsViewError", null,
                        "Can not perform the query. Documents view is missing information."), context);
                return null;
            }

            context.CommerceContext.AddObject(arg);

            string search = arg.Properties
                .FirstOrDefault(p => p.Name.Equals("Term", StringComparison.OrdinalIgnoreCase))?.Value;

            if (string.IsNullOrEmpty(search))
            {
                search = "*";
            }            

            var parameters = new SearchParameters { Select = new[] {"*"} };

            string filterParameter = arg.Properties
                .FirstOrDefault(p => p.Name.Equals("Filter", StringComparison.OrdinalIgnoreCase))?.Value;
            if (!string.IsNullOrEmpty(filterParameter))
            {
                parameters.Filter = filterParameter;
            }

            string orderParameter = arg.Properties.FirstOrDefault(
                p => p.Name.Equals("OrderBy", StringComparison.OrdinalIgnoreCase))?.Value;
            if (!string.IsNullOrEmpty(orderParameter))
            {
                parameters.OrderBy = new[] {orderParameter};
            }

            string skipParameter = arg.Properties
                .FirstOrDefault(p => p.Name.Equals("Skip", StringComparison.OrdinalIgnoreCase))?.Value;
            if (!string.IsNullOrEmpty(skipParameter) && int.TryParse(skipParameter, out var skip))
            {
                parameters.Skip = skip;
            }


            string topParameter = arg.Properties
                .FirstOrDefault(p => p.Name.Equals("Top", StringComparison.OrdinalIgnoreCase))?.Value;
            if (!string.IsNullOrEmpty(topParameter) && int.TryParse(topParameter, out var top))
            {
                parameters.Top = top;
            }

            AlgoliaSearchResultsResponse documentSearchResult = await _searchCommand.QueryDocuments(scopePolicy.Name, search, parameters, context.CommerceContext);

            if (documentSearchResult != null)
            {
                context.CommerceContext.AddObject(documentSearchResult);
            }

            return arg;
        }

        public override Task<EntityView> ContinueTask(EntityView arg, CommercePipelineExecutionContext context)
        {
            return Task.FromResult(arg);
        }
    }
}