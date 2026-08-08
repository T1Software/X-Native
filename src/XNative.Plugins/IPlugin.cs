using XNative.Core.Abstractions;

namespace XNative.Plugins
{
    /// <summary>
    /// Contract implemented by every X-Native plugin.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Stable identifier of the plugin.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Human readable name of the plugin.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Called once by the host after the plugin has been discovered.
        /// </summary>
        /// <param name="host">The environment provided by the application.</param>
        void Initialize(IPluginHost host);
    }
}
