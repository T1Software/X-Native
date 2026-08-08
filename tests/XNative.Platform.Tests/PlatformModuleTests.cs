using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using XNative.Core.Abstractions;
using XNative.Platform;
using Xunit;

namespace XNative.Platform.Tests
{
    public sealed class PlatformModuleTests
    {
        [Fact]
        public void AddXNativePlatformRegistersTheModuleDescriptor()
        {
            using ServiceProvider provider = new ServiceCollection()
                .AddXNativePlatform()
                .BuildServiceProvider();

            IModuleDescriptor descriptor = provider.GetServices<IModuleDescriptor>().Single();

            Assert.Equal("xnative.platform", descriptor.Id);
        }
    }
}
