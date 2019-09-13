using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Search;
using Sitecore.Framework.Pipelines;

namespace Plugin.Commerce.Search.Algolia.Blocks
{
    public class DeleteAllArtifactStoreDocumentsBlock : ConditionalPipelineBlock<bool, bool, CommercePipelineExecutionContext>
    {
        private readonly AlgoliaContextCommand _command;

        public DeleteAllArtifactStoreDocumentsBlock(AlgoliaContextCommand command)
        {
            this._command = command;
            this.BlockCondition = PipelineBlockHelper.ValidatePolicy;
        }


        public override async Task<bool> Run(bool arg, CommercePipelineExecutionContext context)
        {
            if (!arg) return false;

            SearchIndexArgument argument = context.CommerceContext.GetObjects<SearchIndexArgument>().FirstOrDefault();

            if (argument == null)
            {
                context.Abort($"{nameof(Name)}: SearchIndexArgument was not found in the context objects collection.", context);
                return false;
            }

            SearchScopePolicy policyByName = SearchScopePolicy.GetPolicyByName(context.CommerceContext, context.CommerceContext.Environment, argument.IndexName);

            if (policyByName == null)
            {
                context.Abort($"{nameof(Name)}: SearchScopePolicy was not found for index {argument.IndexName}.", context);
                return false;
            }

            IndexablePolicy policyByScope = IndexablePolicy.GetPolicyByScope(context.CommerceContext, context.CommerceContext.Environment, policyByName.Name);
            if (policyByScope?.Properties == null)
            {
                // ISSUE: explicit non-virtual call
                context.Abort($"{nameof(Name)}: IndexablePolicy was not found for index {argument.IndexName}.", context);
                return false;
            }

            return await _command.DeleteAllDocumentsInIndex(context.CommerceContext, argument.IndexName);            
        }

        public override Task<bool> ContinueTask(bool arg, CommercePipelineExecutionContext context)
        {
            return Task.FromResult(arg);
        }
    }
}