using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using XNative.Core.Games;

namespace XNative.Core.Abstractions
{
    /// <summary>
    /// Provides access to the catalog of installed games.
    /// </summary>
    public interface IGameCatalog
    {
        /// <summary>
        /// Gets all games in the catalog.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A read-only collection of games.</returns>
        Task<IReadOnlyCollection<GameInfo>> GetGamesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds a game by its unique identifier.
        /// </summary>
        /// <param name="id">The game identifier.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The game if found; otherwise <c>null</c>.</returns>
        Task<GameInfo?> FindByIdAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds a game by its executable path.
        /// </summary>
        /// <param name="executablePath">The executable path.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The game if found; otherwise <c>null</c>.</returns>
        Task<GameInfo?> FindByExecutablePathAsync(string executablePath, CancellationToken cancellationToken = default);

        /// <summary>
        /// Refreshes the catalog by re-scanning the game sources.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A task representing the refresh operation.</returns>
        Task RefreshAsync(CancellationToken cancellationToken = default);
    }
}