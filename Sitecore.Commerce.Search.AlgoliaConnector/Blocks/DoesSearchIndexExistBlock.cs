using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Search;
using Sitecore.Framework.Conditions;

namespace Plugin.Commerce.Search.Algolia.Blocks
{
    public class DoesSearchIndexExistBlock : ConditionalPipelineBlock<SearchIndexArgument, SearchIndexArgument, CommercePipelineExecutionContext>
    {
        private readonly AlgoliaContextCommand _command;

        public DoesSearchIndexExistBlock(AlgoliaContextCommand command)
        {
            _command = command;
            BlockCondition = PipelineBlockHelper.ValidatePolicy;
        }

        public override async Task<SearchIndexArgument> Run(SearchIndexArgument arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{nameof(Name)}: argument can not be null.");
            context.CommerceContext.AddObject(arg);
            var result = new SearchResultBool(await _command.DoesIndexExist(arg.IndexName, context.CommerceContext));

            if (result.Succeeded == false)
            {
                context.Abort("Index doesnt exists", arg.IndexName);
            }

            context.CommerceContext.AddObject(result);

            return arg;
        }

        public override Task<SearchIndexArgument> ContinueTask(SearchIndexArgument arg, CommercePipelineExecutionContext context)
        {
            return Task.FromResult(arg);
        }
    }
}