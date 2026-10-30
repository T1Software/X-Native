using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Windows.Storage;
using XNative.Core.Abstractions;
using XNative.Platform.Games;

namespace XNative.XboxHost.Services
{
    /// <summary>Registers host services that require the Windows Runtime.</summary>
    public static class XboxHostModule
    {
        /// <summary>Registers Xbox services and an app-local game catalogue root.</summary>
        public static IServiceCollection AddXboxHost(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            services.AddSingleton<IModuleDescriptor, XboxHostModuleDescriptor>();
            services.AddSingleton<IDeviceInfoService, UwpDeviceInfoService>();
            // Bootstrapper registers the platform defaults first; override those here.
            services.Configure<LocalGameCatalogOptions>(options =>
            {
                options.RootPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Games");
            });
            return services;
        }
    }
}
