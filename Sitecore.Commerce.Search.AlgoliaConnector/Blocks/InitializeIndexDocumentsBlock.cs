using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Framework.Conditions;

namespace Plugin.Commerce.Search.Algolia.Blocks
{
    public class InitializeIndexDocumentsBlock : ConditionalPipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly AlgoliaContextCommand _command;

        public InitializeIndexDocumentsBlock(AlgoliaContextCommand command)
        {
            _command = command;
            this.BlockCondition = PipelineBlockHelper.ValidatePolicy;
        }

        public override Task<EntityView> Run(EntityView arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{Name}: argument can not be null.");

            List<JObject> documents = new List<JObject>();

            arg.ChildViews.Cast<EntityView>().ForEach(child =>
            {
                JObject document = new JObject();

                child.Properties.ForEach(property =>
                {
                    if (document.Property(property.Name) != null)
                        document.Property(property.Name).Value = property.RawValue.ToString();
                    else
                        document.Add(property.Name, property.RawValue != null ? JToken.FromObject(property.RawValue) : null);
                });
                documents.Add(document);
            });

            context.CommerceContext.AddObject(documents.AsEnumerable());
            return Task.FromResult(arg);
        }

        public override Task<EntityView> ContinueTask(EntityView arg, CommercePipelineExecutionContext context)
        {
            return Task.FromResult<EntityView>(arg);
        }
    }
}