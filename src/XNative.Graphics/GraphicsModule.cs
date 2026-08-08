using Microsoft.Extensions.DependencyInjection;
using XNative.Core.Abstractions;

namespace XNative.Graphics
{
    /// <summary>
    /// Registration entry point of the graphics module.
    /// </summary>
    public static class GraphicsModule
    {
        /// <summary>
        /// Registers the graphics module services.
        /// </summary>
        /// <param name="services">The service collection to populate.</param>
        /// <returns>The same service collection, to allow chaining.</returns>
        public static IServiceCollection AddXNativeGraphics(this IServiceCollection services)
        {
            services.AddSingleton<IModuleDescriptor, GraphicsModuleDescriptor>();
            return services;
        }
    }
}
