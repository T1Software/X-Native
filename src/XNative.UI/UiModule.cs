using Microsoft.Extensions.DependencyInjection;
using XNative.Core.Abstractions;
using XNative.UI.ViewModels;

namespace XNative.UI
{
    /// <summary>
    /// Registration entry point of the presentation module.
    /// </summary>
    public static class UiModule
    {
        /// <summary>
        /// Registers the presentation module services.
        /// </summary>
        /// <param name="services">The service collection to populate.</param>
        /// <returns>The same service collection, to allow chaining.</returns>
        public static IServiceCollection AddXNativeUI(this IServiceCollection services)
        {
            services.AddSingleton<IModuleDescriptor, UiModuleDescriptor>();
            services.AddTransient<StartupViewModel>();
            services.AddTransient<GameLibraryViewModel>();
            return services;
        }
    }
}
