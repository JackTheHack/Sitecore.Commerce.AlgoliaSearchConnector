using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Plugin.Commerce.Search.Algolia
{
    public static class PipelineBlockHelper
    {
        public static bool ValidatePolicy(IPipelineExecutionContext obj)
        {
            return ((CommercePipelineExecutionContext)obj).CommerceContext.HasPolicy<AlgoliaSearchPolicy>();
        }

        public static AlgoliaSearchPolicy GetAlgoliaPolicy(CommerceContext obj)
        {
            return obj.GetPolicy<AlgoliaSearchPolicy>();
        }
    }
}
