using System.Threading;
using System.Threading.Tasks;
using XNative.Core.Games;

namespace XNative.Core.Abstractions
{
    /// <summary>
    /// Launches a game using the appropriate compatibility layer.
    /// </summary>
    public interface IGameRunner
    {
        /// <summary>
        /// Launches the specified game.
        /// </summary>
        /// <param name="game">The game to launch.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The result of the launch operation.</returns>
        Task<LaunchResult> LaunchAsync(GameInfo game, CancellationToken cancellationToken = default);
    }
}