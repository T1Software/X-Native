using XNative.Core.Abstractions;

namespace XNative.Graphics
{
    /// <summary>
    /// Describes the graphics module.
    /// </summary>
    public sealed class GraphicsModuleDescriptor : IModuleDescriptor
    {
        /// <inheritdoc />
        public string Id => "xnative.graphics";

        /// <inheritdoc />
        public string DisplayName => "Graphics";
    }
}
