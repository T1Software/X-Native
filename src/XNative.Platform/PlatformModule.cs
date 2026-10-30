using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using XNative.Core.Abstractions;
using XNative.Platform.Games;
using XNative.Platform.Storage;

namespace XNative.Platform
{
    /// <summary>Registration entry point of the platform module.</summary>
    public static class PlatformModule
    {
        /// <summary>Registers platform services, preserving a host-supplied filesystem.</summary>
        public static IServiceCollection AddXNativePlatform(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            services.AddSingleton<IModuleDescriptor, PlatformModuleDescriptor>();
            services.TryAddSingleton<IFileSystemService, PhysicalFileSystemService>();
            services.Configure<LocalGameCatalogOptions>(options =>
            {
                options.RootPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            });
            services.AddSingleton<IGameCatalog, LocalGameCatalog>();
            return services;
        }
    }
}
