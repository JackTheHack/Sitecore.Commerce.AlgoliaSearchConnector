using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Search;

namespace Plugin.Commerce.Search.Algolia.Blocks
{
    public class DeleteIndexBlock : ConditionalPipelineBlock<SearchIndexArgument, SearchIndexArgument, CommercePipelineExecutionContext>
    {
        private readonly AlgoliaContextCommand _command;

        public DeleteIndexBlock(AlgoliaContextCommand algoliaCommand)
        {
            _command = algoliaCommand;
            BlockCondition = PipelineBlockHelper.ValidatePolicy;
        }

        public override async Task<SearchIndexArgument> Run(SearchIndexArgument arg, CommercePipelineExecutionContext context)
        {
            await _command.DeleteIndex(arg.IndexName, context.CommerceContext);
            return arg;
        }

        public override Task<SearchIndexArgument> ContinueTask(SearchIndexArgument arg, CommercePipelineExecutionContext context)
        {
            return Task.FromResult(arg);
        }
    }
}