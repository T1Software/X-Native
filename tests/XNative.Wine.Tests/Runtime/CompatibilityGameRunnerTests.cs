using System;
using System.Threading;
using System.Threading.Tasks;
using XNative.Core.Abstractions;
using XNative.Core.Games;
using XNative.Wine.Runtime;
using Xunit;

namespace XNative.Wine.Tests.Runtime
{
    public sealed class CompatibilityGameRunnerTests
    {
        private readonly CompatibilityGameRunner _runner;

        public CompatibilityGameRunnerTests()
        {
            _runner = new CompatibilityGameRunner();
        }

        [Fact]
        public async Task LaunchAsync_ReturnsFailure_ForWindowsExecutable()
        {
            var game = new GameInfo(
                "test-id",
                "Test Game",
                @"C:\Games\Test\game.exe",
                @"C:\Games\Test",
                ExecutableType.WindowsExecutable);

            var result = await _runner.LaunchAsync(game);

            Assert.False(result.Success);
            Assert.Equal("Windows executable detected. Compatibility backend not implemented.", result.Message);
            Assert.Null(result.ProcessId);
        }

        [Fact]
        public async Task LaunchAsync_ReturnsFailure_ForUwpApplication()
        {
            var game = new GameInfo(
                "uwp-id",
                "UWP Game",
                @"C:\Games\UWP\app.exe",
                @"C:\Games\UWP",
                ExecutableType.UwpApplication);

            var result = await _runner.LaunchAsync(game);

            Assert.False(result.Success);
            Assert.Equal("Executable type 'UwpApplication' is not supported.", result.Message);
        }

        [Fact]
        public async Task LaunchAsync_ReturnsFailure_ForUnknownType()
        {
            var game = new GameInfo(
                "unknown-id",
                "Unknown Game",
                @"C:\Games\Unknown\app.exe",
                @"C:\Games\Unknown",
                ExecutableType.Unknown);

            var result = await _runner.LaunchAsync(game);

            Assert.False(result.Success);
            Assert.Equal("Executable type 'Unknown' is not supported.", result.Message);
        }

        [Fact]
        public async Task LaunchAsync_ThrowsOnNullGame()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _runner.LaunchAsync(null!));
        }

        [Fact]
        public async Task LaunchAsync_RespectsCancellationToken()
        {
            var game = new GameInfo(
                "test-id",
                "Test Game",
                @"C:\Games\Test\game.exe",
                @"C:\Games\Test",
                ExecutableType.WindowsExecutable);

            var cts = new CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(() => _runner.LaunchAsync(game, cts.Token));
        }

        [Fact]
        public void LaunchResult_Ok_CreatesSuccessResult()
        {
            var result = LaunchResult.Ok("Launched successfully", 1234);

            Assert.True(result.Success);
            Assert.Equal("Launched successfully", result.Message);
            Assert.Equal(1234, result.ProcessId);
        }

        [Fact]
        public void LaunchResult_Fail_CreatesFailureResult()
        {
            var result = LaunchResult.Fail("Failed to launch");

            Assert.False(result.Success);
            Assert.Equal("Failed to launch", result.Message);
            Assert.Null(result.ProcessId);
        }
    }
}