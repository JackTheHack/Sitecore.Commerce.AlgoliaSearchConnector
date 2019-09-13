using Plugin.Commerce.Search.Algolia.Blocks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Search;
using Sitecore.Framework.Pipelines;
using Sitecore.Framework.Pipelines.Definitions.Extensions;
using ProcessDocumentSearchResultBlock = Sitecore.Commerce.Plugin.Search.ProcessDocumentSearchResultBlock;

namespace Plugin.Commerce.Search.Algolia
{
    public static class AlgoliaPipelineExtensions
    {
        public static SitecorePipelinesConfigBuilder ConfigureAlgoliaDeleteIndex(this SitecorePipelinesConfigBuilder config)
        {
            return config.ConfigurePipeline<IDeleteSearchIndexPipeline>(c =>
                c.Add<DeleteIndexBlock>().Before<ProcessIndexResultBoolBlock>());
        }

        public static SitecorePipelinesConfigBuilder ConfigureAlgoliaSearch(this SitecorePipelinesConfigBuilder config)
        {
            return config.ConfigurePipeline<ISearchPipeline>(c =>
                c.Add<QueryDocumentsBlock>().Before<ProcessDocumentSearchResultBlock>()
                 .Add<ParseQueryTermBlock>().Before<QueryDocumentsBlock>()
                 .Add<CreateFilterListForQueryBlock>().Before<QueryDocumentsBlock>()
                 .Add<ProcessDocumentSearchResultBlock>().After<QueryDocumentsBlock>());
        }

        public static SitecorePipelinesConfigBuilder ConfigureAlgoliaIndexers(
            this SitecorePipelinesConfigBuilder config)
        {
            return config
                .ConfigurePipeline<IPrepareFullIndexMinionPipeline>(c =>                    
                    c.Add<DoesSearchIndexExistBlock>().Before<ProcessIndexResultBoolBlock>()
                     .Add<FullIndexStartedBlock>().Before<DoesSearchIndexExistBlock>()
                     .Add<PrepareAlgoliaIndexBlock>().After<ProcessIndexResultBoolBlock>()
                     .Add<DeleteAllArtifactStoreDocumentsBlock>())
                .ConfigurePipeline<IPrepareIncrementalIndexMinionPipeline>(c =>
                    c.Add<DoesSearchIndexExistBlock>().Before<ProcessIndexResultBoolBlock>()
                     .Add<IncrementalIndexStartedBlock>().Before<DoesSearchIndexExistBlock>()
                     .Add<PrepareAlgoliaIndexBlock>().After<ProcessIndexResultBoolBlock>())
                .ConfigurePipeline<IFullIndexMinionPipeline>(c => c                    
                    .Replace<InitializeCatalogIndexingViewBlock, InitializeAlgoliaCatalogIndexingViewBlock>()
                    .Replace<InitializeCategoryIndexingViewBlock, InitializeAlgoliaCategoryIndexingViewBlock>()
                    .Replace<InitializeSellableItemIndexingViewBlock, InitializeAlgoliaSellableItemIndexingViewBlock>()
                    .Add<FullIndexCompletedBlock>())
                .ConfigurePipeline<IIncrementalIndexMinionPipeline>(c => c
                    .Replace<InitializeCatalogIndexingViewBlock, InitializeAlgoliaCatalogIndexingViewBlock>()
                    .Replace<InitializeCategoryIndexingViewBlock, InitializeAlgoliaCategoryIndexingViewBlock>()
                    .Replace<InitializeSellableItemIndexingViewBlock, InitializeAlgoliaSellableItemIndexingViewBlock>()
                );
        }

        public static SitecorePipelinesConfigBuilder ConfigureAlgoliaDeleteDocuments(this SitecorePipelinesConfigBuilder config)
        {
            return config
                .ConfigurePipeline<IDeleteDocumentsFromIndexPipeline>(c => c
                    .Add<InitializeIndexDocumentsBlock>().Before<ProcessIndexingResultsBlock>()
                    .Add<DeleteIndexDocumentsBlock>().After<InitializeIndexDocumentsBlock>())
                .ConfigurePipeline<IPrepareDeleteIndexDocumentsMinionPipeline>(c => c
                    .Add<DoesSearchIndexExistBlock>().Before<ProcessIndexResultBoolBlock>()
                    .Add<PrepareAlgoliaIndexBlock>().After<ProcessIndexResultBoolBlock>());
        }

        public static SitecorePipelinesConfigBuilder ConfigureAlgoliaAddDocuments(
            this SitecorePipelinesConfigBuilder config)
        {
            return config
                .ConfigurePipeline<IAddDocumentsToIndexPipeline>(c => c
                    .Add<InitializeIndexDocumentsBlock>().Before<ProcessIndexingResultsBlock>()
                    .Add<MergeOrAddIndexDocumentsBlock>().After<InitializeIndexDocumentsBlock>());
        }

        public static SitecorePipelinesConfigBuilder ConfigureAlgoliaListIndex(
            this SitecorePipelinesConfigBuilder config)
        {
            return config
                .ConfigurePipeline<IGetSearchIndexPipeline>(c =>
                    c.Add<GetIndexBlock>().Before<ProcessGetIndexResultBlock>())
                .ConfigurePipeline<IListSearchIndexesPipeline>(c =>
                    c.Add<ListIndexesBlock>().Before<ProcessListIndexesResultBlock>());
        }
    }   
}
