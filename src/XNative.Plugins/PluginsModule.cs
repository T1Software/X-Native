using Microsoft.Extensions.DependencyInjection;
using XNative.Core.Abstractions;

namespace XNative.Plugins
{
    /// <summary>
    /// Registration entry point of the plugins module.
    /// </summary>
    public static class PluginsModule
    {
        /// <summary>
        /// Registers the plugins module services.
        /// </summary>
        /// <param name="services">The service collection to populate.</param>
        /// <returns>The same service collection, to allow chaining.</returns>
        public static IServiceCollection AddXNativePlugins(this IServiceCollection services)
        {
            services.AddSingleton<IModuleDescriptor, PluginsModuleDescriptor>();
            return services;
        }
    }
}
