using XNative.Core.Abstractions;

namespace XNative.XboxHost.Services
{
    /// <summary>
    /// Describes the Xbox host module.
    /// </summary>
    public sealed class XboxHostModuleDescriptor : IModuleDescriptor
    {
        /// <inheritdoc />
        public string Id => "xnative.xboxhost";

        /// <inheritdoc />
        public string DisplayName => "Xbox Host";
    }
}
