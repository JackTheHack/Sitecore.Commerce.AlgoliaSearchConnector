using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Search;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using CatalogItemBase = Sitecore.Commerce.Plugin.Catalog.CatalogItemBase;

namespace Plugin.Commerce.Search.Algolia.Blocks
{
    public abstract class InitializeIndexingViewBlockBase<T> : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext> where T: CatalogItemBase
    {
        protected CommercePipelineExecutionContext CommerceContext;

        public virtual List<T> GetEntities(SearchIndexMinionArgument indexArgument)
        {
            return indexArgument.Entities?
                .OfType<T>()
                .ToList();
        }

        public abstract void BuildCustomProperties(T entity, EntityView view);

        public virtual void BuildProperties(T entity, EntityView view)
        {
            var banner = entity.Id.Contains("NW") ? "NW" : "PNS";

            view.Properties
                //.AddOrUpdateProperty("EntityId", entity.Id)
                .AddProperty("Banner", banner)                
                .AddProperty("Type", entity.GetType().Name)
                .AddOrUpdateProperty("EntityVersion", entity.EntityVersion)
                .AddProperty("SitecoreId", entity.SitecoreId)
                .AddProperty("DisplayName", entity.DisplayName)
                .AddProperty("Name", entity.Name)
                .AddProperty("objectID", entity.Id)
                .AddProperty("Published", entity.Published)
                .AddProperty("DateCreated", entity.DateCreated)
                .AddProperty("DateUpdated", entity.DateUpdated)
                .AddProperty("ArtifactStoreId", CommerceContext.CommerceContext.Environment.ArtifactStoreId);
        }
        
        public override Task<EntityView> Run(EntityView arg, CommercePipelineExecutionContext context)
        {
            CommerceContext = context;

            Condition.Requires(arg).IsNotNull($"{Name}: argument cannot be null.");

            SearchIndexMinionArgument indexMinionArgument =
                CommerceContext.CommerceContext
                    .GetObjects<SearchIndexMinionArgument>()
                    .FirstOrDefault();

            if (string.IsNullOrEmpty(indexMinionArgument?.Policy?.Name))
            {
                return Task.FromResult(arg);
            }

            var source = GetEntities(indexMinionArgument);

            if (source == null || !source.Any())
            {
                return Task.FromResult(arg);
            }

            KnownSearchViewsPolicy searchViewNames = context.GetPolicy<KnownSearchViewsPolicy>();
            source.ForEach(si =>
            {
                EntityView entityView = arg.ChildViews
                    .Cast<EntityView>()
                    .FirstOrDefault(v => v.EntityId.Equals(si.Id, StringComparison.OrdinalIgnoreCase) && v.Name.Equals(searchViewNames.Document, StringComparison.OrdinalIgnoreCase));

                if (entityView == null)
                {
                    entityView = new EntityView
                    {
                        Name = context.GetPolicy<KnownSearchViewsPolicy>().Document,
                        EntityId = si.Id,
                        EntityVersion = si.EntityVersion
                    };                   

                    arg.ChildViews.Add(entityView);
                }

                BuildProperties(si, entityView);

                BuildCustomProperties(si, entityView);
            });   
            
            //context.Logger.LogInformation($"Processed {source.Count} {typeof(T)} items.");

            return Task.FromResult(arg);
        }
    }

    internal static class ViewPropertyListExtensions
    {
        public static List<ViewProperty> AddProperty(this List<ViewProperty> properties, string key, object value)
        {
            properties.Add(new ViewProperty
            {
                Name = key,
                RawValue = value
            });
            return properties;
        }

        public static List<ViewProperty> AddOrUpdateProperty(this List<ViewProperty> properties, string key, object value)
        {
            var property = properties.FirstOrDefault(i => i.Name == key);
            if (property != null)
            {
                property.RawValue = value;
                return properties;
            }

            return AddProperty(properties, key, value);
        }       
    }
}
