using System;
using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Search;
using Sitecore.Framework.Pipelines;

namespace Plugin.Commerce.Search.Algolia.Blocks
{
    public class PrepareAlgoliaIndexBlock : ConditionalPipelineBlock<bool, bool, CommercePipelineExecutionContext>
    {
        private readonly IInitializeSearchIndexPipeline _initIndexPipeline;

        public PrepareAlgoliaIndexBlock(IInitializeSearchIndexPipeline initIndexPipeline)
        {
            _initIndexPipeline = initIndexPipeline;
            this.BlockCondition = new Predicate<IPipelineExecutionContext>(PipelineBlockHelper.ValidatePolicy);
        }

        public override Task<bool> ContinueTask(bool arg, CommercePipelineExecutionContext context)
        {
            return Task.FromResult<bool>(arg);
        }

        public override async Task<bool> Run(bool arg, CommercePipelineExecutionContext context)
        {
            if (arg) return true;

            SearchIndexArgument argument = context.CommerceContext.GetObjects<SearchIndexArgument>().FirstOrDefault();

            if (argument == null)
            {
                return true;
            }

            var result = await _initIndexPipeline.Run(argument, context);

            if (result != null)
            {
                return true;
            }

            CommercePipelineExecutionContext executionContext = context;
            CommerceContext commerceContext = context.CommerceContext;
            string error = context.GetPolicy<KnownResultCodes>().Error;
            string commerceTermKey = "CreateIndexError";
            object[] args = {argument.IndexName};
            string defaultMessage = $"Search index '{argument.IndexName as object}' was not created.";
            executionContext.Abort(await commerceContext.AddMessage(error, commerceTermKey, args, defaultMessage), context);
            return false;
        }
    }
}