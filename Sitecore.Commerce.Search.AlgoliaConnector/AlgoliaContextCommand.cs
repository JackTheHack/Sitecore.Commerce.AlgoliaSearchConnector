using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Algolia.Search;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Commerce.Search.Algolia.Models;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Search;
using Sitecore.Framework.Pipelines;

namespace Plugin.Commerce.Search.Algolia
{
    public class AlgoliaContextCommand : CommerceCommand
    {
        private AlgoliaClient _algoliaIndexingClient;
        private AlgoliaClient _algoliaSearchClient;

        private AlgoliaClient CreateIndexingClient(CommerceContext context)
        {
            if (_algoliaIndexingClient != null)
            {
                return _algoliaIndexingClient;
            }

            var algoliaPolicy = PipelineBlockHelper.GetAlgoliaPolicy(context);
            _algoliaIndexingClient = new AlgoliaClient(algoliaPolicy.ApplicationId, algoliaPolicy.WriteApiKey);
            return _algoliaIndexingClient;
        }

        private AlgoliaClient CreateSearchClient(CommerceContext context)
        {
            if (_algoliaSearchClient != null)
            {
                return _algoliaSearchClient;
            }

            var algoliaPolicy = PipelineBlockHelper.GetAlgoliaPolicy(context);
            _algoliaSearchClient = new AlgoliaClient(algoliaPolicy.ApplicationId, algoliaPolicy.SearchApiKey);
            return _algoliaSearchClient;
        }

        public async Task<bool> DoesIndexExist(string indexName, CommerceContext commerceContext)
        {
            var client = CreateIndexingClient(commerceContext);
            dynamic indexes = await client.ListIndexesAsync();
            JArray indexesArray = indexes["items"];
            bool indexExists = indexesArray.Any(i => i["name"].ToObject<string>() == indexName);
            return indexExists;
        }      

        public async Task<bool> DeleteIndex(string argIndexName, CommerceContext context)
        {
            var client = CreateIndexingClient(context);
            var result = await client.DeleteIndexAsync(argIndexName);
            return result.Property("taskID") != null;
        }

        public Index GetSearchIndex(string argIndexName, CommerceContext contextCommerceContext)
        {
            var client = CreateIndexingClient(contextCommerceContext);
            return client.InitIndex(argIndexName);
        }

        public async Task<ListIndexesResponse> ListSearchIndexesAsync(CommerceContext contextCommerceContext)
        {
            var client = CreateIndexingClient(contextCommerceContext);
            var indexes = await client.ListIndexesAsync();

            return indexes.ToObject<ListIndexesResponse>();
        }

        public async Task<bool> DeleteAllDocumentsInIndex(CommerceContext context, string argumentIndexName)
        {
            var client = CreateIndexingClient(context);
            var index = client.InitIndex(argumentIndexName);

            var response = await index.ClearIndexAsync();
            return response.Property("taskID") != null;
        }

        public async Task<bool> DeleteDocuments(string name, IEnumerable<string> documentArray, CommerceContext contextCommerceContext)
        {
            var client = CreateIndexingClient(contextCommerceContext);
            var index = client.InitIndex(name);

            var response = await index.DeleteObjectsAsync(documentArray);
            return response.Property("taskID") != null;
        }

        public async Task<AlgoliaSearchResultsResponse> QueryDocuments(string name, string search, SearchParameters parameters, CommerceContext contextCommerceContext)
        {
            var client = CreateSearchClient(contextCommerceContext);
            var index = client.InitIndex(name);
            var response = await index.SearchAsync(new Query(search));
            return response.ToObject<AlgoliaSearchResultsResponse>();
        }

        public async Task<IEnumerable<IndexingResult>> MergeOrAddDocuments(string name, IEnumerable<JObject> documentArray, CommerceContext contextCommerceContext)
        {
            var client = CreateIndexingClient(contextCommerceContext);
            var index = client.InitIndex(name);
            var response = await index.SaveObjectsAsync(documentArray);

            var objectIds = response.Property("objectIDs")?.Value?.ToObject<string[]>();

            return objectIds?.Select(i => new IndexingResult(
                i.StartsWith("Entity-") ? i : $"Entity-SellableItem-{i}", 
                null, true)).ToList();
        }
    }
}
