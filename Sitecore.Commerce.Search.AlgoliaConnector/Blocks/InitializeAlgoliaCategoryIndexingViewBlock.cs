using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Search;

namespace Plugin.Commerce.Search.Algolia.Blocks
{
    public class InitializeAlgoliaCategoryIndexingViewBlock : InitializeIndexingViewBlockBase<Category>
    {
        public override void BuildCustomProperties(Category entity, EntityView view)
        {
            
        }
    }
}
