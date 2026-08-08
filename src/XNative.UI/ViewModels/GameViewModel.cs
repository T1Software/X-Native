using System;
using XNative.Core.Games;
using XNative.UI.Mvvm;

namespace XNative.UI.ViewModels
{
    /// <summary>
    /// View model representing a single game in the library.
    /// </summary>
    public sealed class GameViewModel : ObservableObject
    {
        private readonly GameInfo _gameInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameViewModel"/> class.
        /// </summary>
        /// <param name="gameInfo">The game information.</param>
        public GameViewModel(GameInfo gameInfo)
        {
            _gameInfo = gameInfo ?? throw new ArgumentNullException(nameof(gameInfo));
        }

        /// <summary>
        /// The game's display name.
        /// </summary>
        public string Name => _gameInfo.Name;

        /// <summary>
        /// The full path to the executable.
        /// </summary>
        public string ExecutablePath => _gameInfo.ExecutablePath;

        /// <summary>
        /// The runner identifier (e.g., "wine", "uwp", "native").
        /// </summary>
        public string Runner => _gameInfo.ExecutableType switch
        {
            ExecutableType.WindowsExecutable => "wine",
            ExecutableType.UwpApplication => "uwp",
            _ => "unknown"
        };

        /// <summary>
        /// The executable type.
        /// </summary>
        public ExecutableType ExecutableType => _gameInfo.ExecutableType;

        /// <summary>
        /// Display text combining name and runner.
        /// </summary>
        public string DisplayText => $"{Name} ({Runner})";

        /// <summary>
        /// Detailed text for tooltips or expanded view.
        /// </summary>
        public string DetailText => $"Path: {ExecutablePath}\nRunner: {Runner}";
    }
}