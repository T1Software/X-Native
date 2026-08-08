using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XNative.Core.Abstractions;
using XNative.Core.Games;
using XNative.UI.Mvvm;

namespace XNative.UI.ViewModels
{
    /// <summary>
    /// View model for the game library screen.
    /// </summary>
    public sealed class GameLibraryViewModel : ViewModelBase
    {
        private readonly IGameCatalog _gameCatalog;
        private readonly ObservableCollection<GameViewModel> _games;
        private string _scanStatus = "Ready";
        private int _totalGames;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameLibraryViewModel"/> class.
        /// </summary>
        /// <param name="gameCatalog">The game catalog service.</param>
        public GameLibraryViewModel(IGameCatalog gameCatalog)
        {
            _gameCatalog = gameCatalog ?? throw new ArgumentNullException(nameof(gameCatalog));
            _games = new ObservableCollection<GameViewModel>();
            ScanGamesCommand = new RelayCommand(async _ => await ScanGamesAsync(), _ => !IsBusy);
        }

        /// <summary>
        /// Collection of discovered games.
        /// </summary>
        public ObservableCollection<GameViewModel> Games => _games;

        /// <summary>
        /// Current scan status text.
        /// </summary>
        public string ScanStatus
        {
            get => _scanStatus;
            private set => SetProperty(ref _scanStatus, value);
        }

        /// <summary>
        /// Total number of games found.
        /// </summary>
        public int TotalGames
        {
            get => _totalGames;
            private set
            {
                if (SetProperty(ref _totalGames, value))
                {
                    OnPropertyChanged(nameof(GamesFoundText));
                }
            }
        }

        /// <summary>
        /// Text showing games found count.
        /// </summary>
        public string GamesFoundText => TotalGames > 0
            ? $"{TotalGames} game{(TotalGames == 1 ? "" : "s")} found"
            : "No games found";

        /// <summary>
        /// Command to trigger a game scan.
        /// </summary>
        public RelayCommand ScanGamesCommand { get; }

        /// <summary>
        /// Scans for games and updates the collection.
        /// </summary>
        public async Task ScanGamesAsync(CancellationToken cancellationToken = default)
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;
            ScanStatus = "Scanning...";
            _games.Clear();
            TotalGames = 0;

            try
            {
                var games = await _gameCatalog.GetGamesAsync(cancellationToken);

                foreach (var game in games.OrderBy(g => g.Name, StringComparer.OrdinalIgnoreCase))
                {
                    _games.Add(new GameViewModel(game));
                }

                TotalGames = _games.Count;
                ScanStatus = TotalGames > 0 ? $"Found {TotalGames} game(s)" : "No games found";
            }
            catch (OperationCanceledException)
            {
                ScanStatus = "Scan cancelled";
            }
            catch (Exception ex)
            {
                ScanStatus = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
                ScanGamesCommand.RaiseCanExecuteChanged();
            }
        }
    }
}