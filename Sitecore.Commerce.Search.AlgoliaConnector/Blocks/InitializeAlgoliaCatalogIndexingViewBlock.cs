using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Search;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Plugin.Commerce.Search.Algolia.Blocks
{
    public class InitializeAlgoliaCatalogIndexingViewBlock : InitializeIndexingViewBlockBase<Catalog>
    {
        public override void BuildCustomProperties(Catalog entity, EntityView view)
        {
            
        }
    }
}
