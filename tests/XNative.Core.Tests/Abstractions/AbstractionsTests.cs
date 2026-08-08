using System;
using System.Linq;
using System.Reflection;
using XNative.Core.Abstractions;
using Xunit;

namespace XNative.Core.Tests.Abstractions
{
    public sealed class AbstractionsTests
    {
        [Fact]
        public void CoreHasNoProjectOrPackageDependencies()
        {
            AssemblyName[] references = typeof(IModuleDescriptor).Assembly.GetReferencedAssemblies();

            Assert.DoesNotContain(references, reference => reference.Name!.StartsWith("XNative.", StringComparison.Ordinal));
            Assert.DoesNotContain(references, reference => reference.Name!.StartsWith("Microsoft.Extensions.", StringComparison.Ordinal));
        }

        [Fact]
        public void EveryAbstractionIsAPublicInterface()
        {
            Type[] abstractions = typeof(IModuleDescriptor).Assembly
                .GetTypes()
                .Where(type => type.Namespace == "XNative.Core.Abstractions")
                .ToArray();

            Assert.NotEmpty(abstractions);
            Assert.All(abstractions, type =>
            {
                Assert.True(type.IsInterface, $"{type.Name} must be an interface.");
                Assert.True(type.IsPublic, $"{type.Name} must be public.");
            });
        }
    }
}
