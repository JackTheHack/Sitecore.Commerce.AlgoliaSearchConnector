using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Search;
using Sitecore.Framework.Configuration;
using Sitecore.Framework.Pipelines.Definitions.Extensions;

namespace Plugin.Commerce.Search.Algolia
{
    public class ConfigureSitecore : IConfigureSitecore
    {
        public void ConfigureServices(IServiceCollection services)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(executingAssembly);
            services.RegisterAllCommands(executingAssembly);
            services.Sitecore().Pipelines(config =>
            {
                config
                .ConfigureAlgoliaListIndex()
                .ConfigureAlgoliaDeleteIndex()
                .ConfigureAlgoliaSearch()
                .ConfigureAlgoliaIndexers()                
                .ConfigureAlgoliaDeleteDocuments()
                .ConfigureAlgoliaAddDocuments()                
                .ConfigurePipeline<IRunningPluginsPipeline>(c => 
                         c.Add<RegisteredPluginBlock>()
                          .After<RunningPluginsBlock>());
            });
        }
    }    
}
