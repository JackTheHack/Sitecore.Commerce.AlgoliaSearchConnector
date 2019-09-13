using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Search;
using Sitecore.Framework.Conditions;

namespace Plugin.Commerce.Search.Algolia.Blocks
{
    public class CreateFilterListForQueryBlock : ConditionalPipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public CreateFilterListForQueryBlock()
        {
            this.BlockCondition = PipelineBlockHelper.ValidatePolicy;
        }

        public override async Task<EntityView> Run(EntityView arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires<EntityView>(arg).IsNotNull($"{Name}: argument can not be null.");
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

            ICollection<string> retreivableFields = GetRetreivableFields(
                IndexablePolicy.GetPolicyByScope(context.CommerceContext, context.CommerceContext.Environment, scopePolicy.Name));
            if (retreivableFields != null && retreivableFields.Any<string>())
                context.CommerceContext.AddObject((object)retreivableFields);
            return arg;
        }

        private ICollection<string> GetRetreivableFields(IndexablePolicy indexPolicy)
        {
            List<string> stringList = new List<string>();
            if (indexPolicy == null)
            {
                return stringList;
            }

            stringList.AddRange(indexPolicy.Properties.Select(field => new
            {
                field,
                fieldName = field.Key.ToLower()
            })
            .Where(x => x.field.Value.IsRetrievable)
            .Select(x => x.fieldName));

            return stringList;
        }

        public override Task<EntityView> ContinueTask(EntityView arg, CommercePipelineExecutionContext context)
        {
            return Task.FromResult(arg);
        }
    }
}