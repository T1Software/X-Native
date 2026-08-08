using XNative.Core.Abstractions;

namespace XNative.Platform
{
    /// <summary>
    /// Describes the platform module.
    /// </summary>
    public sealed class PlatformModuleDescriptor : IModuleDescriptor
    {
        /// <inheritdoc />
        public string Id => "xnative.platform";

        /// <inheritdoc />
        public string DisplayName => "Platform";
    }
}
