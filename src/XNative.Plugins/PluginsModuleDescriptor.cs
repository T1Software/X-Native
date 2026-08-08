using XNative.Core.Abstractions;

namespace XNative.Plugins
{
    /// <summary>
    /// Describes the plugins module.
    /// </summary>
    public sealed class PluginsModuleDescriptor : IModuleDescriptor
    {
        /// <inheritdoc />
        public string Id => "xnative.plugins";

        /// <inheritdoc />
        public string DisplayName => "Plugins";
    }
}
