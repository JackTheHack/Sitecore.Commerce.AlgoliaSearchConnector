using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Catalog;

namespace Plugin.Commerce.Search.Algolia.Blocks
{
    public class InitializeAlgoliaSellableItemIndexingViewBlock : InitializeIndexingViewBlockBase<SellableItem>
    {
        private readonly IConfiguration _configuration;

        public InitializeAlgoliaSellableItemIndexingViewBlock(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override void BuildCustomProperties(SellableItem entity, EntityView view)
        {
               
        }
    }
}
