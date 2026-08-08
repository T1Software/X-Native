using System;
using XNative.Core.Games;
using Xunit;

namespace XNative.Core.Tests.Games
{
    public sealed class ExecutableTypeTests
    {
        [Fact]
        public void ExecutableType_HasExpectedValues()
        {
            Assert.Equal(0, (int)ExecutableType.Unknown);
            Assert.Equal(1, (int)ExecutableType.WindowsExecutable);
            Assert.Equal(2, (int)ExecutableType.UwpApplication);
        }

        [Fact]
        public void ExecutableType_CanBeParsedFromString()
        {
            Assert.True(Enum.TryParse("Unknown", out ExecutableType _));
            Assert.True(Enum.TryParse("WindowsExecutable", out ExecutableType _));
            Assert.True(Enum.TryParse("UwpApplication", out ExecutableType _));
        }
    }

    public sealed class GameMetadataTests
    {
        [Fact]
        public void GameMetadata_DefaultsToNull()
        {
            var metadata = new GameMetadata();
            Assert.Null(metadata.Description);
            Assert.Null(metadata.Publisher);
            Assert.Null(metadata.Version);
            Assert.Null(metadata.ReleaseDate);
            Assert.Null(metadata.IconPath);
        }

        [Fact]
        public void GameMetadata_CanBeInitializedWithValues()
        {
            var releaseDate = new DateTime(2023, 1, 15);
            var metadata = new GameMetadata(
                "Test game",
                "Test Publisher",
                "1.0.0",
                releaseDate,
                "icon.png");

            Assert.Equal("Test game", metadata.Description);
            Assert.Equal("Test Publisher", metadata.Publisher);
            Assert.Equal("1.0.0", metadata.Version);
            Assert.Equal(releaseDate, metadata.ReleaseDate);
            Assert.Equal("icon.png", metadata.IconPath);
        }
    }

    public sealed class GameInfoTests
    {
        [Fact]
        public void GameInfo_CreatesSuccessfullyWithRequiredParameters()
        {
            var info = new GameInfo(
                "test-id",
                "Test Game",
                @"C:\Games\Test\game.exe",
                @"C:\Games\Test",
                ExecutableType.WindowsExecutable);

            Assert.Equal("test-id", info.Id);
            Assert.Equal("Test Game", info.Name);
            Assert.Equal(@"C:\Games\Test\game.exe", info.ExecutablePath);
            Assert.Equal(@"C:\Games\Test", info.WorkingDirectory);
            Assert.Equal(ExecutableType.WindowsExecutable, info.ExecutableType);
            Assert.Null(info.Metadata);
        }

        [Fact]
        public void GameInfo_CreatesSuccessfullyWithMetadata()
        {
            var metadata = new GameMetadata("Description", "Publisher", "1.0");
            var info = new GameInfo(
                "test-id",
                "Test Game",
                @"C:\Games\Test\game.exe",
                @"C:\Games\Test",
                ExecutableType.WindowsExecutable,
                metadata);

            Assert.NotNull(info.Metadata);
            Assert.Equal("Description", info.Metadata.Description);
            Assert.Equal("Publisher", info.Metadata.Publisher);
            Assert.Equal("1.0", info.Metadata.Version);
        }

        [Fact]
        public void GameInfo_ThrowsOnNullId()
        {
            Assert.Throws<ArgumentNullException>(() => new GameInfo(
                null!,
                "Test",
                @"C:\test.exe",
                @"C:\",
                ExecutableType.WindowsExecutable));
        }

        [Fact]
        public void GameInfo_ThrowsOnEmptyId()
        {
            Assert.Throws<ArgumentException>(() => new GameInfo(
                "",
                "Test",
                @"C:\test.exe",
                @"C:\",
                ExecutableType.WindowsExecutable));
        }

        [Fact]
        public void GameInfo_ThrowsOnWhitespaceId()
        {
            Assert.Throws<ArgumentException>(() => new GameInfo(
                "   ",
                "Test",
                @"C:\test.exe",
                @"C:\",
                ExecutableType.WindowsExecutable));
        }

        [Fact]
        public void GameInfo_ThrowsOnNullName()
        {
            Assert.Throws<ArgumentNullException>(() => new GameInfo(
                "id",
                null!,
                @"C:\test.exe",
                @"C:\",
                ExecutableType.WindowsExecutable));
        }

        [Fact]
        public void GameInfo_ThrowsOnNullExecutablePath()
        {
            Assert.Throws<ArgumentNullException>(() => new GameInfo(
                "id",
                "Test",
                null!,
                @"C:\",
                ExecutableType.WindowsExecutable));
        }

        [Fact]
        public void GameInfo_ThrowsOnNullWorkingDirectory()
        {
            Assert.Throws<ArgumentNullException>(() => new GameInfo(
                "id",
                "Test",
                @"C:\test.exe",
                null!,
                ExecutableType.WindowsExecutable));
        }

        [Fact]
        public void GameInfo_AllowsUwpApplicationType()
        {
            var info = new GameInfo(
                "uwp-id",
                "UWP Game",
                @"C:\Games\UWP\app.exe",
                @"C:\Games\UWP",
                ExecutableType.UwpApplication);

            Assert.Equal(ExecutableType.UwpApplication, info.ExecutableType);
        }

        [Fact]
        public void GameInfo_AllowsUnknownType()
        {
            var info = new GameInfo(
                "unknown-id",
                "Unknown Game",
                @"C:\Games\Unknown\app.exe",
                @"C:\Games\Unknown",
                ExecutableType.Unknown);

            Assert.Equal(ExecutableType.Unknown, info.ExecutableType);
        }
    }
}