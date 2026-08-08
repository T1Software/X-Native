using System;
using Microsoft.Extensions.DependencyInjection;
using XNative.Core.Abstractions;

namespace XNative.XboxHost.Services
{
    /// <summary>
    /// Registration entry point of the Xbox host module. It supplies the
    /// implementations that require the Windows Runtime.
    /// </summary>
    public static class XboxHostModule
    {
        /// <summary>
        /// Registers the host services.
        /// </summary>
        /// <param name="services">The service collection to populate.</param>
        /// <returns>The same service collection, to allow chaining.</returns>
        public static IServiceCollection AddXboxHost(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<IModuleDescriptor, XboxHostModuleDescriptor>();
            services.AddSingleton<IDeviceInfoService, UwpDeviceInfoService>();
            return services;
        }
    }
}
