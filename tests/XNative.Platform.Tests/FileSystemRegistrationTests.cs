using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using XNative.Core.Abstractions;
using XNative.Platform;
using XNative.Platform.Storage;
using Xunit;

namespace XNative.Platform.Tests
{
    public sealed class FileSystemRegistrationTests
    {
        [Fact]
        public void PlatformRegistrationResolvesCatalogueWithValidation()
        {
            var services = new ServiceCollection();
            services.AddXNativePlatform();
            using var provider = services.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true
            });
            Assert.IsType<PhysicalFileSystemService>(provider.GetRequiredService<IFileSystemService>());
            Assert.NotNull(provider.GetRequiredService<IGameCatalog>());
        }

        [Fact]
        public void PlatformRegistrationPreservesHostFilesystem()
        {
            var filesystem = new Mock<IFileSystemService>().Object;
            var services = new ServiceCollection();
            services.AddSingleton(filesystem);
            services.AddXNativePlatform();
            using var provider = services.BuildServiceProvider();
            Assert.Same(filesystem, provider.GetRequiredService<IFileSystemService>());
        }

        [Fact]
        public void PlatformRegistrationRejectsNullCollection()
        {
            Assert.Throws<ArgumentNullException>(() => PlatformModule.AddXNativePlatform(null!));
        }
    }

    public sealed class PhysicalFileSystemServiceTests : IDisposable
    {
        private readonly string _root = Path.Combine(Path.GetTempPath(), "xnative-" + Guid.NewGuid().ToString("N"));
        private readonly PhysicalFileSystemService _filesystem = new PhysicalFileSystemService();

        public PhysicalFileSystemServiceTests() => Directory.CreateDirectory(_root);

        [Fact]
        public void EnumeratesOnlyImmediateFilesAndDirectories()
        {
            var nested = Path.Combine(_root, "nested");
            Directory.CreateDirectory(nested);
            var exe = Path.Combine(_root, "game.exe");
            File.WriteAllText(exe, "header fixture");
            File.WriteAllText(Path.Combine(_root, "notes.txt"), "notes");
            File.WriteAllText(Path.Combine(nested, "child.exe"), "nested fixture");
            Assert.True(_filesystem.DirectoryExists(_root));
            Assert.True(_filesystem.FileExists(exe));
            Assert.Equal(exe, Assert.Single(_filesystem.GetFiles(_root, "*.exe")));
            Assert.Equal(nested, Assert.Single(_filesystem.GetDirectories(_root)));
        }

        [Fact]
        public async Task ReadsUnicodeTextAndClosesTheFile()
        {
            var path = Path.Combine(_root, "game.json");
            var text = "{\"name\":\"Grüße 🎮\"}";
            File.WriteAllText(path, text, new UTF8Encoding(true));
            Assert.Equal(text, await _filesystem.ReadAllTextAsync(path));
            using var exclusive = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            Assert.True(exclusive.CanWrite);
        }

        [Fact]
        public async Task CancellationIsCheckedBeforeOpeningTheFile()
        {
            using var source = new CancellationTokenSource();
            source.Cancel();
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                _filesystem.ReadAllTextAsync(Path.Combine(_root, "missing.json"), source.Token));
        }

        [Fact]
        public async Task MissingFileErrorsAreNotHidden()
        {
            await Assert.ThrowsAsync<FileNotFoundException>(() =>
                _filesystem.ReadAllTextAsync(Path.Combine(_root, "missing.json")));
        }

        public void Dispose() => Directory.Delete(_root, recursive: true);
    }
}
