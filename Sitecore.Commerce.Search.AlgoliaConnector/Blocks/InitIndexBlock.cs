using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Search;

namespace Plugin.Commerce.Search.Algolia.Blocks
{
    public class InitIndexBlock : ConditionalPipelineBlock<SearchIndexArgument, SearchIndexArgument, CommercePipelineExecutionContext>
    {
        private readonly AlgoliaContextCommand _command;

        public InitIndexBlock(AlgoliaContextCommand algolia)
        {
            _command = algolia;
            BlockCondition = PipelineBlockHelper.ValidatePolicy;
        }

        public override Task<SearchIndexArgument> Run(SearchIndexArgument arg, CommercePipelineExecutionContext context)
        {
            //if (!await _command.DoesIndexExist(arg.IndexName, context.CommerceContext))
            //{
            //    CommercePipelineExecutionContext executionContext = context;
            //    CommerceContext commerceContext = context.CommerceContext;
            //    string error = context.GetPolicy<KnownResultCodes>().Error;
            //    string commerceTermKey = "IndexDoesNotExist";
            //    object[] args = new object[1]{arg.IndexName};
            //    string defaultMessage = $"Search Index '{(object) arg.IndexName}' does not exist.";
            //    executionContext.Abort(await commerceContext.AddMessage(error, commerceTermKey, args, defaultMessage), (object)context);
            //    return null;
            //}

            //var index = _command.InitIndex(arg.IndexName, context.CommerceContext);

            //if (index == null)
            //{
              //  context.Abort($"Could not init index '{arg.IndexName}'", context);
            //}

            return Task.FromResult(arg);
        }

        public override Task<SearchIndexArgument> ContinueTask(SearchIndexArgument arg, CommercePipelineExecutionContext context)
        {
            return Task.FromResult(arg);
        }
    }
}