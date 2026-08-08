using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using XNative.Core.Abstractions;
using XNative.Core.Games;

namespace XNative.Platform.Games
{
    /// <summary>
    /// Configuration options for the local game catalog.
    /// </summary>
    public sealed class LocalGameCatalogOptions
    {
        /// <summary>
        /// The root directory to scan for games.
        /// </summary>
        public string RootPath { get; set; } = string.Empty;

        /// <summary>
        /// File patterns to consider as game executables.
        /// </summary>
        public string[] ExecutablePatterns { get; set; } = { "*.exe" };

        /// <summary>
        /// Maximum directory depth to scan.
        /// </summary>
        public int MaxDepth { get; set; } = 3;
    }

    /// <summary>
    /// Represents a game manifest file (game.json).
    /// </summary>
    public sealed class GameManifest
    {
        /// <summary>
        /// Display name of the game.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Executable file name relative to the manifest directory.
        /// </summary>
        public string? Executable { get; set; }

        /// <summary>
        /// Optional working directory relative to the manifest directory.
        /// </summary>
        public string? WorkingDirectory { get; set; }

        /// <summary>
        /// Optional description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Optional publisher.
        /// </summary>
        public string? Publisher { get; set; }

        /// <summary>
        /// Optional version.
        /// </summary>
        public string? Version { get; set; }
    }

    /// <summary>
    /// Local file system based game catalog implementation.
    /// </summary>
    public sealed class LocalGameCatalog : IGameCatalog
    {
        private readonly LocalGameCatalogOptions _options;
        private readonly IFileSystemService _fileSystem;
        private readonly object _lock = new();
        private IReadOnlyCollection<GameInfo>? _cachedGames;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalGameCatalog"/> class.
        /// </summary>
        /// <param name="options">Catalog configuration options.</param>
        /// <param name="fileSystem">File system abstraction.</param>
        public LocalGameCatalog(IOptions<LocalGameCatalogOptions> options, IFileSystemService fileSystem)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<GameInfo>> GetGamesAsync(CancellationToken cancellationToken = default)
        {
            if (_cachedGames != null)
            {
                return _cachedGames;
            }

            await RefreshAsync(cancellationToken);
            return _cachedGames ?? Array.Empty<GameInfo>();
        }

        /// <inheritdoc />
        public async Task<GameInfo?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            var games = await GetGamesAsync(cancellationToken);
            return games.FirstOrDefault(g => string.Equals(g.Id, id, StringComparison.OrdinalIgnoreCase));
        }

        /// <inheritdoc />
        public async Task<GameInfo?> FindByExecutablePathAsync(string executablePath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(executablePath))
            {
                return null;
            }

            var games = await GetGamesAsync(cancellationToken);
            return games.FirstOrDefault(g => string.Equals(g.ExecutablePath, executablePath, StringComparison.OrdinalIgnoreCase));
        }

        /// <inheritdoc />
        public async Task RefreshAsync(CancellationToken cancellationToken = default)
        {
            var games = new List<GameInfo>();

            if (!_fileSystem.DirectoryExists(_options.RootPath))
            {
                lock (_lock)
                {
                    _cachedGames = games;
                }
                return;
            }

            await ScanDirectoryAsync(_options.RootPath, 0, games, cancellationToken);

            lock (_lock)
            {
                _cachedGames = games;
            }
        }

        private async Task ScanDirectoryAsync(string directory, int depth, List<GameInfo> games, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (depth > _options.MaxDepth)
            {
                return;
            }

            try
            {
                // First, check for game.json manifest
                var manifestPath = Path.Combine(directory, "game.json");
                bool manifestProcessed = false;
                if (_fileSystem.FileExists(manifestPath))
                {
                    var gameInfo = await TryCreateGameFromManifestAsync(manifestPath, directory, cancellationToken);
                    if (gameInfo != null)
                    {
                        games.Add(gameInfo);
                        manifestProcessed = true;
                    }
                }

                // Then scan for executables, but skip if a manifest was successfully processed
                if (!manifestProcessed)
                {
                    foreach (var pattern in _options.ExecutablePatterns)
                    {
                        var executables = _fileSystem.GetFiles(directory, pattern);
                        foreach (var exePath in executables)
                        {
                            var gameInfo = CreateGameFromExecutable(exePath);
                            if (gameInfo != null && !games.Any(g => string.Equals(g.ExecutablePath, exePath, StringComparison.OrdinalIgnoreCase)))
                            {
                                games.Add(gameInfo);
                            }
                        }
                    }
                }

                // Recurse into subdirectories
                var subDirs = _fileSystem.GetDirectories(directory);
                foreach (var subDir in subDirs)
                {
                    await ScanDirectoryAsync(subDir, depth + 1, games, cancellationToken);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Skip directories we can't access
            }
            catch (IOException)
            {
                // Skip directories with IO errors
            }
        }

        private async Task<GameInfo?> TryCreateGameFromManifestAsync(string manifestPath, string directory, CancellationToken cancellationToken)
        {
            try
            {
                var json = await _fileSystem.ReadAllTextAsync(manifestPath, cancellationToken);
                var manifest = JsonSerializer.Deserialize<GameManifest>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (manifest == null || string.IsNullOrWhiteSpace(manifest.Executable))
                {
                    return null;
                }

                var exePath = Path.Combine(directory, manifest.Executable);
                if (!_fileSystem.FileExists(exePath))
                {
                    return null;
                }

                var workingDir = string.IsNullOrWhiteSpace(manifest.WorkingDirectory)
                    ? directory
                    : Path.Combine(directory, manifest.WorkingDirectory);

                var metadata = new GameMetadata(
                    manifest.Description,
                    manifest.Publisher,
                    manifest.Version,
                    null,
                    null);

                var id = GenerateId(manifest.Name ?? Path.GetFileNameWithoutExtension(manifest.Executable), exePath);
                return new GameInfo(
                    id,
                    manifest.Name ?? Path.GetFileNameWithoutExtension(manifest.Executable),
                    exePath,
                    workingDir,
                    ExecutableType.WindowsExecutable,
                    metadata);
            }
            catch (JsonException)
            {
                return null;
            }
            catch (IOException)
            {
                return null;
            }
        }

        private GameInfo? CreateGameFromExecutable(string exePath)
        {
            try
            {
                var name = Path.GetFileNameWithoutExtension(exePath);
                var workingDir = Path.GetDirectoryName(exePath) ?? string.Empty;
                var id = GenerateId(name, exePath);

                return new GameInfo(
                    id,
                    name,
                    exePath,
                    workingDir,
                    ExecutableType.WindowsExecutable,
                    null);
            }
            catch
            {
                return null;
            }
        }

        private static string GenerateId(string name, string exePath)
        {
            var pathHash = exePath.GetHashCode().ToString("x8");
            return $"{name.ToLowerInvariant().Replace(" ", "-")}-{pathHash}";
        }
    }
}