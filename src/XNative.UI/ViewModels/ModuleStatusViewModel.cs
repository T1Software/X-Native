using System;
using XNative.UI.Mvvm;

namespace XNative.UI.ViewModels
{
    /// <summary>
    /// Presents the load state of a single module.
    /// </summary>
    public sealed class ModuleStatusViewModel : ObservableObject
    {
        private bool _isLoaded;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleStatusViewModel"/> class.
        /// </summary>
        /// <param name="id">The identifier of the module.</param>
        /// <param name="displayName">The human readable name of the module.</param>
        /// <param name="isLoaded">Whether the module was resolved from the container.</param>
        public ModuleStatusViewModel(string id, string displayName, bool isLoaded)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            _isLoaded = isLoaded;
        }

        /// <summary>
        /// Identifier of the module.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Human readable name of the module.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Indicates whether the module was resolved from the container.
        /// </summary>
        public bool IsLoaded
        {
            get => _isLoaded;
            set
            {
                if (SetProperty(ref _isLoaded, value))
                {
                    OnPropertyChanged(nameof(StatusText));
                    OnPropertyChanged(nameof(Line));
                }
            }
        }

        /// <summary>
        /// Load state rendered as text.
        /// </summary>
        public string StatusText => IsLoaded ? "Loaded \u2713" : "Missing \u2717";

        /// <summary>
        /// The complete line as shown on the startup screen.
        /// </summary>
        public string Line => DisplayName + ": " + StatusText;
    }
}
