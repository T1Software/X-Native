using XNative.Core.Abstractions;

namespace XNative.Wine
{
    /// <summary>
    /// Describes the compatibility layer module.
    /// </summary>
    public sealed class WineModuleDescriptor : IModuleDescriptor
    {
        /// <inheritdoc />
        public string Id => "xnative.wine";

        /// <inheritdoc />
        public string DisplayName => "Compatibility Layer";
    }
}
