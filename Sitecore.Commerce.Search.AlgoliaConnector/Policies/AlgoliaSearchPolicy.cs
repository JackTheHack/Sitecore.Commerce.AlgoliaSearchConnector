using Sitecore.Commerce.Core;

namespace Plugin.Commerce.Search.Algolia
{
    public class AlgoliaSearchPolicy : Policy
    {
        public string SearchApiKey { get; set; }
        public string WriteApiKey { get; set; }        
        public string ApplicationId { get; set; }
        public bool DefaultSearchOnlyStringFields { get; set; }
    }
}
