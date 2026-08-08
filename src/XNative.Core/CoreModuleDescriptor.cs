using XNative.Core.Abstractions;

namespace XNative.Core
{
    /// <summary>
    /// Describes the core module. The core cannot register itself because it
    /// has no dependency on a container, so the composition root registers
    /// this descriptor explicitly.
    /// </summary>
    public sealed class CoreModuleDescriptor : IModuleDescriptor
    {
        /// <inheritdoc />
        public string Id => "xnative.core";

        /// <inheritdoc />
        public string DisplayName => "Core";
    }
}
