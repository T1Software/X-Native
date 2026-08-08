using System;
using System.Threading;
using System.Threading.Tasks;
using XNative.Core.Abstractions;
using XNative.Core.Games;

namespace XNative.Wine.Runtime
{
    /// <summary>
    /// Stub implementation of IGameRunner that detects Windows executables
    /// but does not yet implement actual compatibility layer execution.
    /// </summary>
    public sealed class CompatibilityGameRunner : IGameRunner
    {
        /// <inheritdoc />
        public Task<LaunchResult> LaunchAsync(GameInfo game, CancellationToken cancellationToken = default)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }

            cancellationToken.ThrowIfCancellationRequested();

            if (game.ExecutableType == ExecutableType.WindowsExecutable)
            {
                return Task.FromResult(LaunchResult.Fail(
                    "Windows executable detected. Compatibility backend not implemented."));
            }

            return Task.FromResult(LaunchResult.Fail(
                $"Executable type '{game.ExecutableType}' is not supported."));
        }
    }
}