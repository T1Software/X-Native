using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using XNative.Core.Abstractions;
using XNative.Core.Games;
using XNative.Platform.Games;
using Xunit;

namespace XNative.Platform.Tests.Games
{
    public sealed class LocalGameCatalogTests
    {
        private readonly Mock<IFileSystemService> _mockFileSystem;
        private readonly LocalGameCatalogOptions _options;
        private readonly LocalGameCatalog _catalog;
        private readonly string _testRoot;

        public LocalGameCatalogTests()
        {
            _mockFileSystem = new Mock<IFileSystemService>();
            _options = new LocalGameCatalogOptions
            {
                RootPath = @"C:\TestGames",
                ExecutablePatterns = new[] { "*.exe" },
                MaxDepth = 3
            };
            _testRoot = _options.RootPath;
            _catalog = new LocalGameCatalog(Options.Create(_options), _mockFileSystem.Object);
        }

        [Fact]
        public async Task GetGamesAsync_ReturnsEmpty_WhenRootDoesNotExist()
        {
            _mockFileSystem.Setup(fs => fs.DirectoryExists(_testRoot)).Returns(false);

            var games = await _catalog.GetGamesAsync();

            Assert.Empty(games);
        }

        [Fact]
        public async Task GetGamesAsync_DetectsExeFiles()
        {
            var exePath = Path.Combine(_testRoot, "Game1", "game1.exe");
            var dirPath = Path.GetDirectoryName(exePath)!;

            _mockFileSystem.Setup(fs => fs.DirectoryExists(_testRoot)).Returns(true);
            _mockFileSystem.Setup(fs => fs.GetDirectories(_testRoot)).Returns(new[] { dirPath });
            _mockFileSystem.Setup(fs => fs.GetDirectories(dirPath)).Returns(Array.Empty<string>());
            _mockFileSystem.Setup(fs => fs.GetFiles(dirPath, "*.exe")).Returns(new[] { exePath });
            _mockFileSystem.Setup(fs => fs.FileExists(exePath)).Returns(true);

            var games = await _catalog.GetGamesAsync();

            Assert.Single(games);
            var game = games.First();
            Assert.Equal("game1", game.Name);
            Assert.Equal(exePath, game.ExecutablePath);
            Assert.Equal(dirPath, game.WorkingDirectory);
            Assert.Equal(ExecutableType.WindowsExecutable, game.ExecutableType);
        }

        [Fact]
        public async Task GetGamesAsync_DetectsGamesFromManifest()
        {
            var manifestPath = Path.Combine(_testRoot, "Game2", "game.json");
            var exePath = Path.Combine(_testRoot, "Game2", "launcher.exe");
            var dirPath = Path.GetDirectoryName(manifestPath)!;

            var manifestJson = """{"name": "Manifest Game", "executable": "launcher.exe"}""";

            _mockFileSystem.Setup(fs => fs.DirectoryExists(_testRoot)).Returns(true);
            _mockFileSystem.Setup(fs => fs.GetDirectories(_testRoot)).Returns(new[] { dirPath });
            _mockFileSystem.Setup(fs => fs.GetDirectories(dirPath)).Returns(Array.Empty<string>());
            _mockFileSystem.Setup(fs => fs.FileExists(manifestPath)).Returns(true);
            _mockFileSystem.Setup(fs => fs.ReadAllTextAsync(manifestPath, It.IsAny<CancellationToken>()))
                .ReturnsAsync(manifestJson);
            _mockFileSystem.Setup(fs => fs.FileExists(exePath)).Returns(true);
            _mockFileSystem.Setup(fs => fs.GetFiles(dirPath, "*.exe")).Returns(Array.Empty<string>());

            var games = await _catalog.GetGamesAsync();

            Assert.Single(games);
            var game = games.First();
            Assert.Equal("Manifest Game", game.Name);
            Assert.Equal(exePath, game.ExecutablePath);
            Assert.Equal(dirPath, game.WorkingDirectory);
        }

        [Fact]
        public async Task GetGamesAsync_ManifestTakesPrecedenceOverExe()
        {
            var manifestPath = Path.Combine(_testRoot, "Game3", "game.json");
            var exePath = Path.Combine(_testRoot, "Game3", "game3.exe");
            var dirPath = Path.GetDirectoryName(manifestPath)!;

            var manifestJson = """{"name": "Manifest Game", "executable": "launcher.exe"}""";

            _mockFileSystem.Setup(fs => fs.DirectoryExists(_testRoot)).Returns(true);
            _mockFileSystem.Setup(fs => fs.GetDirectories(_testRoot)).Returns(new[] { dirPath });
            _mockFileSystem.Setup(fs => fs.GetDirectories(dirPath)).Returns(Array.Empty<string>());
            _mockFileSystem.Setup(fs => fs.FileExists(manifestPath)).Returns(true);
            _mockFileSystem.Setup(fs => fs.ReadAllTextAsync(manifestPath, It.IsAny<CancellationToken>()))
                .ReturnsAsync(manifestJson);
            _mockFileSystem.Setup(fs => fs.FileExists(exePath)).Returns(true);
            _mockFileSystem.Setup(fs => fs.FileExists(Path.Combine(dirPath, "launcher.exe"))).Returns(true);
            _mockFileSystem.Setup(fs => fs.GetFiles(dirPath, "*.exe")).Returns(new[] { exePath });

            var games = await _catalog.GetGamesAsync();

            Assert.Single(games);
            Assert.Equal("Manifest Game", games.First().Name);
        }

        [Fact]
        public async Task FindByIdAsync_ReturnsGame_WhenExists()
        {
            var exePath = Path.Combine(_testRoot, "Game1", "game1.exe");
            var dirPath = Path.GetDirectoryName(exePath)!;

            _mockFileSystem.Setup(fs => fs.DirectoryExists(_testRoot)).Returns(true);
            _mockFileSystem.Setup(fs => fs.GetDirectories(_testRoot)).Returns(new[] { dirPath });
            _mockFileSystem.Setup(fs => fs.GetDirectories(dirPath)).Returns(Array.Empty<string>());
            _mockFileSystem.Setup(fs => fs.GetFiles(dirPath, "*.exe")).Returns(new[] { exePath });
            _mockFileSystem.Setup(fs => fs.FileExists(exePath)).Returns(true);

            var games = await _catalog.GetGamesAsync();

            var found = await _catalog.FindByIdAsync(games.First().Id);

            Assert.NotNull(found);
            Assert.Equal(exePath, found.ExecutablePath);
        }

        [Fact]
        public async Task FindByIdAsync_ReturnsNull_WhenNotExists()
        {
            _mockFileSystem.Setup(fs => fs.DirectoryExists(_testRoot)).Returns(false);

            var found = await _catalog.FindByIdAsync("nonexistent");

            Assert.Null(found);
        }

        [Fact]
        public async Task FindByExecutablePathAsync_ReturnsGame_WhenExists()
        {
            var exePath = Path.Combine(_testRoot, "Game1", "game1.exe");
            var dirPath = Path.GetDirectoryName(exePath)!;

            _mockFileSystem.Setup(fs => fs.DirectoryExists(_testRoot)).Returns(true);
            _mockFileSystem.Setup(fs => fs.GetDirectories(_testRoot)).Returns(new[] { dirPath });
            _mockFileSystem.Setup(fs => fs.GetDirectories(dirPath)).Returns(Array.Empty<string>());
            _mockFileSystem.Setup(fs => fs.GetFiles(dirPath, "*.exe")).Returns(new[] { exePath });
            _mockFileSystem.Setup(fs => fs.FileExists(exePath)).Returns(true);

            await _catalog.GetGamesAsync();

            var found = await _catalog.FindByExecutablePathAsync(exePath);

            Assert.NotNull(found);
            Assert.Equal(exePath, found.ExecutablePath);
        }

        [Fact]
        public async Task RefreshAsync_UpdatesCache()
        {
            var exePath = Path.Combine(_testRoot, "Game1", "game1.exe");
            var dirPath = Path.GetDirectoryName(exePath)!;

            _mockFileSystem.Setup(fs => fs.DirectoryExists(_testRoot)).Returns(true);
            _mockFileSystem.Setup(fs => fs.GetDirectories(_testRoot)).Returns(new[] { dirPath });
            _mockFileSystem.Setup(fs => fs.GetDirectories(dirPath)).Returns(Array.Empty<string>());
            _mockFileSystem.Setup(fs => fs.GetFiles(dirPath, "*.exe")).Returns(new[] { exePath });
            _mockFileSystem.Setup(fs => fs.FileExists(exePath)).Returns(true);

            await _catalog.RefreshAsync();
            var firstScan = await _catalog.GetGamesAsync();
            Assert.Single(firstScan);

            // Add another game
            var exePath2 = Path.Combine(_testRoot, "Game2", "game2.exe");
            var dirPath2 = Path.GetDirectoryName(exePath2)!;

            _mockFileSystem.Setup(fs => fs.GetDirectories(_testRoot)).Returns(new[] { dirPath, dirPath2 });
            _mockFileSystem.Setup(fs => fs.GetFiles(dirPath2, "*.exe")).Returns(new[] { exePath2 });
            _mockFileSystem.Setup(fs => fs.FileExists(exePath2)).Returns(true);

            await _catalog.RefreshAsync();
            var secondScan = await _catalog.GetGamesAsync();
            Assert.Equal(2, secondScan.Count);
        }

        [Fact]
        public void GameManifest_DeserializesFromJson()
        {
            var json = """{"name": "Test Game", "executable": "game.exe", "workingDirectory": ".", "description": "A test", "publisher": "TestPub", "version": "1.0"}""";
            var manifest = System.Text.Json.JsonSerializer.Deserialize<GameManifest>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(manifest);
            Assert.Equal("Test Game", manifest.Name);
            Assert.Equal("game.exe", manifest.Executable);
            Assert.Equal(".", manifest.WorkingDirectory);
            Assert.Equal("A test", manifest.Description);
            Assert.Equal("TestPub", manifest.Publisher);
            Assert.Equal("1.0", manifest.Version);
        }
    }
}