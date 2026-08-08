using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using XNative.Core.Abstractions;
using XNative.Platform.Games;

namespace XNative.Platform
{
    /// <summary>
    /// Registration entry point of the platform module.
    /// </summary>
    public static class PlatformModule
    {
        /// <summary>
        /// Registers the platform module services.
        /// </summary>
        /// <param name="services">The service collection to populate.</param>
        /// <returns>The same service collection, to allow chaining.</returns>
        public static IServiceCollection AddXNativePlatform(this IServiceCollection services)
        {
            services.AddSingleton<IModuleDescriptor, PlatformModuleDescriptor>();
            services.Configure<LocalGameCatalogOptions>(options =>
            {
                options.RootPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            });
            services.AddSingleton<IGameCatalog, LocalGameCatalog>();
            return services;
        }
    }
}
