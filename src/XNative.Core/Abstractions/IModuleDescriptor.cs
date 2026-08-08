namespace XNative.Core.Abstractions
{
    /// <summary>
    /// Identifies a module of the platform so that the composition root can
    /// report which modules were loaded. Every module assembly is expected to
    /// provide exactly one implementation.
    /// </summary>
    public interface IModuleDescriptor
    {
        /// <summary>
        /// Stable identifier of the module, for example <c>xnative.platform</c>.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Human readable name of the module.
        /// </summary>
        string DisplayName { get; }
    }
}
