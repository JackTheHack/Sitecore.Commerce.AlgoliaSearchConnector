using System;
using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Search;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Plugin.Commerce.Search.Algolia.Blocks
{
    public class ParseQueryTermBlock : ConditionalPipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public ParseQueryTermBlock()
        {
            this.BlockCondition = PipelineBlockHelper.ValidatePolicy;
        }

        public override async Task<EntityView> Run(EntityView arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{Name}: argument can not be null.");
            CommercePipelineExecutionContext executionContext;

            if (!arg.HasPolicy<SearchScopePolicy>())
            {
                executionContext = context;
                executionContext.Abort(await context.CommerceContext.AddMessage(context.GetPolicy<KnownResultCodes>().Error, "DocumentsViewError", (object[])null, "Can not perform the query. Documents view is missing information."), (object)context);
                return null;
            }

            SearchScopePolicy scopePolicy = arg.GetPolicy<SearchScopePolicy>();
            if (string.IsNullOrEmpty(scopePolicy.Name))
            {
                executionContext = context;
                executionContext.Abort(await context.CommerceContext.AddMessage(context.GetPolicy<KnownResultCodes>().Error, "DocumentsViewError", (object[])null, "Can not perform the query. Documents view is missing information."), (object)context);
                return null;
            }

            ViewProperty viewProperty = arg.Properties.FirstOrDefault(p => p.Name.Equals("Term", StringComparison.OrdinalIgnoreCase));

            if (viewProperty == null)
            {
                return arg;
            }

            IndexablePolicy policyByScope = IndexablePolicy.GetPolicyByScope(context.CommerceContext, context.CommerceContext.Environment, scopePolicy.Name);
            viewProperty.Value = GetParsedTerm(viewProperty.Value, policyByScope, context.GetPolicy<AlgoliaSearchPolicy>().DefaultSearchOnlyStringFields);
            return arg;
        }

        private string GetParsedTerm(string term, IndexablePolicy indexPolicy, object defaultSearchOnlyStringFields)
        {
            if (string.IsNullOrEmpty(term) || term.Equals("*", StringComparison.Ordinal) || indexPolicy == null)
                return "*";
            if (term.Contains(":"))
                return term;

            return $"_text_:{term}";
        }

        public override Task<EntityView> ContinueTask(EntityView arg, CommercePipelineExecutionContext context)
        {
            return Task.FromResult(arg);
        }
    }
}