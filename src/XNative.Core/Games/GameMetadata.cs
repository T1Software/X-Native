using System;

namespace XNative.Core.Games
{
    /// <summary>
    /// Optional metadata about a game.
    /// </summary>
    public sealed class GameMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameMetadata"/> class.
        /// </summary>
        /// <param name="description">Description of the game.</param>
        /// <param name="publisher">Publisher of the game.</param>
        /// <param name="version">Version of the game.</param>
        /// <param name="releaseDate">Release date of the game.</param>
        /// <param name="iconPath">Path to the game's icon.</param>
        public GameMetadata(
            string? description = null,
            string? publisher = null,
            string? version = null,
            DateTime? releaseDate = null,
            string? iconPath = null)
        {
            Description = description;
            Publisher = publisher;
            Version = version;
            ReleaseDate = releaseDate;
            IconPath = iconPath;
        }

        /// <summary>
        /// Description of the game.
        /// </summary>
        public string? Description { get; }

        /// <summary>
        /// Publisher of the game.
        /// </summary>
        public string? Publisher { get; }

        /// <summary>
        /// Version of the game.
        /// </summary>
        public string? Version { get; }

        /// <summary>
        /// Release date of the game.
        /// </summary>
        public DateTime? ReleaseDate { get; }

        /// <summary>
        /// Path to the game's icon.
        /// </summary>
        public string? IconPath { get; }
    }
}