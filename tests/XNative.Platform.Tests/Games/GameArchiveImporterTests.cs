using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using XNative.Platform.Games;
using Xunit;

namespace XNative.Platform.Tests.Games
{
    public sealed class GameArchiveImporterTests : IDisposable
    {
        private readonly string _root = Path.Combine(Path.GetTempPath(), "xnative-import-" + Guid.NewGuid().ToString("N"));
        private string Target => Path.Combine(_root, "game");
        public GameArchiveImporterTests() => Directory.CreateDirectory(_root);

        [Fact]
        public void ExtractsFilesAndLeavesInputOpen()
        {
            using var input = Archive("game/game.exe", "fixture");
            Assert.Equal(1, GameArchiveImporter.Extract(input, Target));
            Assert.Equal("fixture", File.ReadAllText(Path.Combine(Target, "game", "game.exe")));
            Assert.True(input.CanRead);
        }

        [Theory]
        [InlineData("../escape.exe")]
        [InlineData("..\\escape.exe")]
        [InlineData("/absolute.exe")]
        [InlineData("C:/absolute.exe")]
        [InlineData("game/../escape.exe")]
        [InlineData("game/file.exe:stream")]
        [InlineData("game./file.exe")]
        [InlineData("game/CON.txt")]
        [InlineData("AUX")]
        [InlineData("LPT1.bin")]
        public void RejectsUnsafePathsAndRemovesPartialOutput(string path)
        {
            using var input = Archive(path, "fixture");
            Assert.Throws<InvalidDataException>(() => GameArchiveImporter.Extract(input, Target));
            Assert.False(Directory.Exists(Target));
        }

        [Fact]
        public void RejectsOversizedExtraction()
        {
            using var input = Archive("game.exe", "12345");
            Assert.Throws<InvalidDataException>(() => GameArchiveImporter.Extract(input, Target, maxBytes: 4));
            Assert.False(Directory.Exists(Target));
        }

        [Fact]
        public void RejectsCaseInsensitiveDuplicates()
        {
            using var input = Archive("game.exe", "one", "GAME.EXE");
            Assert.Throws<InvalidDataException>(() => GameArchiveImporter.Extract(input, Target));
            Assert.False(Directory.Exists(Target));
        }

        [Fact]
        public void RejectsTooManyEntries()
        {
            using var input = Archive("one.exe", "one", "two.exe");
            Assert.Throws<InvalidDataException>(() => GameArchiveImporter.Extract(input, Target, maxEntries: 1));
        }

        [Fact]
        public void DoesNotOverwriteExistingDestination()
        {
            Directory.CreateDirectory(Target);
            File.WriteAllText(Path.Combine(Target, "keep.txt"), "keep");
            using var input = Archive("game.exe", "fixture");
            Assert.Throws<IOException>(() => GameArchiveImporter.Extract(input, Target));
            Assert.Equal("keep", File.ReadAllText(Path.Combine(Target, "keep.txt")));
        }

        [Fact]
        public void RejectsSymbolicLinks()
        {
            using var input = new MemoryStream();
            using (var archive = new ZipArchive(input, ZipArchiveMode.Create, leaveOpen: true))
                archive.CreateEntry("link").ExternalAttributes = unchecked((int)0xa1ff0000);
            input.Position = 0;
            Assert.Throws<InvalidDataException>(() => GameArchiveImporter.Extract(input, Target));
        }

        [Fact]
        public void HonoursPreCancelledToken()
        {
            using var input = Archive("game.exe", "fixture");
            using var source = new CancellationTokenSource();
            source.Cancel();
            Assert.ThrowsAny<OperationCanceledException>(() => GameArchiveImporter.Extract(input, Target, source.Token));
            Assert.False(Directory.Exists(Target));
        }

        private static MemoryStream Archive(string name, string content, string? second = null)
        {
            var stream = new MemoryStream();
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
            {
                using (var writer = new StreamWriter(archive.CreateEntry(name).Open())) writer.Write(content);
                if (second != null)
                    using (var writer = new StreamWriter(archive.CreateEntry(second).Open())) writer.Write(content);
            }
            stream.Position = 0;
            return stream;
        }
        public void Dispose() => Directory.Delete(_root, recursive: true);
    }
}
