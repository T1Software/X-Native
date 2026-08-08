using Microsoft.Extensions.DependencyInjection;
using XNative.Core;
using XNative.Core.Abstractions;
using XNative.Graphics;
using XNative.Platform;
using XNative.Plugins;
using XNative.UI;
using XNative.Wine;

namespace XNative.App
{
    /// <summary>
    /// Builds the dependency injection container of the application. The
    /// application head, console or UWP, only calls into this type.
    /// </summary>
    public static class CompositionRoot
    {
        /// <summary>
        /// Registers every module of the application.
        /// </summary>
        /// <param name="services">The service collection to populate.</param>
        /// <returns>The same service collection, to allow chaining.</returns>
        public static IServiceCollection AddXNative(this IServiceCollection services)
        {
            services.AddSingleton<IModuleDescriptor, CoreModuleDescriptor>();
            services.AddXNativePlatform();
            services.AddXNativeGraphics();
            services.AddXNativeWine();
            services.AddXNativePlugins();
            services.AddXNativeUI();
            return services;
        }

        /// <summary>
        /// Builds a service provider containing every module.
        /// </summary>
        /// <returns>The service provider of the application.</returns>
        public static ServiceProvider Build()
        {
            return new ServiceCollection()
                .AddXNative()
                .BuildServiceProvider();
        }
    }
}
