using XNative.Core.Abstractions;

namespace XNative.UI
{
    /// <summary>
    /// Describes the presentation module.
    /// </summary>
    public sealed class UiModuleDescriptor : IModuleDescriptor
    {
        /// <inheritdoc />
        public string Id => "xnative.ui";

        /// <inheritdoc />
        public string DisplayName => "Presentation";
    }
}
