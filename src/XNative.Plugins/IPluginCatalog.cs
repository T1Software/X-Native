using System.Collections.Generic;

namespace XNative.Plugins
{
    /// <summary>
    /// Source of the plugins available to the application. Discovery
    /// strategies are provided by the platform module.
    /// </summary>
    public interface IPluginCatalog
    {
        /// <summary>
        /// Returns the plugins currently known to the catalog.
        /// </summary>
        /// <returns>The known plugins.</returns>
        IReadOnlyCollection<IPlugin> GetPlugins();
    }
}
