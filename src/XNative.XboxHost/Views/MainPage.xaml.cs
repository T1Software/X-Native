using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using XNative.Core.Abstractions;
using XNative.Platform.Games;
using XNative.UI.ViewModels;
using XNative.XboxHost.Bootstrap;

namespace XNative.XboxHost.Views
{
    /// <summary>Startup report, library and read-only USB ZIP browser/importer.</summary>
    public sealed partial class MainPage : Page
    {
        private readonly List<StorageFile> _usbArchives = new List<StorageFile>();
        private readonly ComboBox _usbChoice = new ComboBox { MinWidth = 400, MaxWidth = 1000 };
        private readonly Button _usbScan = new Button { Content = "Scan USB storage", IsEnabled = false };
        private readonly Button _usbImport = new Button { Content = "Import selected ZIP", IsEnabled = false };
        private readonly TextBlock _usbStatus = new TextBlock { TextWrapping = TextWrapping.Wrap, FontSize = 20,
            Foreground = new SolidColorBrush(Colors.White), Margin = new Thickness(0, 12, 0, 12) };
        private CancellationTokenSource? _usbCancellation;

        public MainPage()
        {
            InitializeComponent();
            VersionText.Text = "Version " + ReadPackageVersion();
            var panel = (StackPanel)((ScrollViewer)Content).Content;
            panel.Children.Add(new TextBlock { Text = "USB game storage", FontSize = 32,
                Foreground = new SolidColorBrush(Colors.White), Margin = new Thickness(0, 32, 0, 12) });
            _usbStatus.Text = "Place ZIP packages in USB:/XNative/Games. Import copies files locally; it does not enable PC-game execution.";
            panel.Children.Add(_usbStatus);
            panel.Children.Add(_usbScan);
            panel.Children.Add(_usbChoice);
            panel.Children.Add(_usbImport);
            _usbScan.Click += ScanUsb_Click;
            _usbImport.Click += ImportUsb_Click;
            _usbChoice.SelectionChanged += (_, __) => _usbImport.IsEnabled =
                Bootstrapper.IsStarted && _usbCancellation == null && _usbChoice.SelectedIndex >= 0;
            Unloaded += (_, __) => _usbCancellation?.Cancel();
        }

        public StartupViewModel? ViewModel { get; private set; }
        public GameLibraryViewModel? GameLibrary { get; private set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            StartupReport? report = App.Startup;
            if (report == null || !report.IsSuccess) { ShowError(report?.Error); return; }
            ViewModel = Bootstrapper.Resolve<StartupViewModel>();
            ViewModel.Load();
            GameLibrary = Bootstrapper.Resolve<GameLibraryViewModel>();
            _usbScan.IsEnabled = true;
            Bindings.Update();
        }

        private async void ScanUsb_Click(object sender, RoutedEventArgs e)
        {
            if (_usbCancellation != null) return;
            using var cancellation = new CancellationTokenSource();
            _usbCancellation = cancellation;
            SetUsbBusy(true);
            _usbArchives.Clear();
            _usbChoice.Items.Clear();
            var warnings = new List<string>();
            try
            {
                _usbStatus.Text = "Scanning removable storage...";
                var drives = await KnownFolders.RemovableDevices.GetFoldersAsync();
                foreach (var drive in drives)
                {
                    cancellation.Token.ThrowIfCancellationRequested();
                    try
                    {
                        var native = await drive.TryGetItemAsync("XNative") as StorageFolder;
                        var games = native == null ? null : await native.TryGetItemAsync("Games") as StorageFolder;
                        if (games == null) { warnings.Add(drive.Name + ": no XNative/Games folder."); continue; }
                        uint offset = 0;
                        while (true)
                        {
                            cancellation.Token.ThrowIfCancellationRequested();
                            var batch = await games.GetFilesAsync(Windows.Storage.Search.CommonFileQuery.DefaultQuery, offset, 100);
                            foreach (var file in batch)
                            {
                                if (!string.Equals(file.FileType, ".zip", StringComparison.OrdinalIgnoreCase)) continue;
                                _usbArchives.Add(file);
                                _usbChoice.Items.Add(drive.Name + " / " + file.Name);
                                if (_usbArchives.Count >= 500) break;
                            }
                            if (batch.Count < 100 || _usbArchives.Count >= 500) break;
                            offset += (uint)batch.Count;
                        }
                    }
                    catch (OperationCanceledException) { throw; }
                    catch (Exception ex) { warnings.Add(drive.Name + ": " + ex.Message); }
                    if (_usbArchives.Count >= 500) { warnings.Add("Display limited to 500 ZIP packages."); break; }
                }
                if (_usbArchives.Count > 0) _usbChoice.SelectedIndex = 0;
                _usbStatus.Text = drives.Count + " device(s), " + _usbArchives.Count + " ZIP package(s). " + string.Join(" ", warnings);
            }
            catch (OperationCanceledException) { _usbStatus.Text = "USB scan cancelled."; }
            catch (Exception ex) { _usbStatus.Text = "USB access failed: " + ex.Message; }
            finally { _usbCancellation = null; SetUsbBusy(false); }
        }

        private async void ImportUsb_Click(object sender, RoutedEventArgs e)
        {
            var index = _usbChoice.SelectedIndex;
            if (_usbCancellation != null || index < 0 || index >= _usbArchives.Count) return;
            using var cancellation = new CancellationTokenSource();
            _usbCancellation = cancellation;
            SetUsbBusy(true);
            string? staging = null;
            try
            {
                var selected = _usbArchives[index];
                var local = ApplicationData.Current.LocalFolder.Path;
                var id = Guid.NewGuid().ToString("N");
                var extractionPath = Path.Combine(local, "Imports", id);
                staging = extractionPath;
                var destination = Path.Combine(local, "Games", "usb-" + id);
                _usbStatus.Text = "Importing " + selected.Name + " (up to 4 GiB / 10000 entries)...";
                int count;
                using (var input = await selected.OpenStreamForReadAsync())
                    count = await Task.Run(() => GameArchiveImporter.Extract(input, extractionPath, cancellation.Token));
                cancellation.Token.ThrowIfCancellationRequested();
                Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
                Directory.Move(extractionPath, destination);
                staging = null;
                _usbStatus.Text = "Imported " + count + " file(s). Source USB unchanged. Local folder: " + destination;
                await Bootstrapper.Resolve<IGameCatalog>().RefreshAsync(cancellation.Token);
                if (GameLibrary != null) await GameLibrary.ScanGamesAsync(cancellation.Token);
            }
            catch (OperationCanceledException) { _usbStatus.Text += " Operation cancelled; completed imports are retained."; }
            catch (Exception ex) { _usbStatus.Text += " Import/scan failed: " + ex.Message; }
            finally
            {
                if (staging != null && Directory.Exists(staging))
                {
                    try { Directory.Delete(staging, recursive: true); }
                    catch (Exception ex) { _usbStatus.Text += " Staging cleanup failed: " + ex.Message; }
                }
                _usbCancellation = null;
                SetUsbBusy(false);
            }
        }

        private void SetUsbBusy(bool busy)
        {
            _usbScan.IsEnabled = !busy && Bootstrapper.IsStarted;
            _usbChoice.IsEnabled = !busy;
            _usbImport.IsEnabled = !busy && Bootstrapper.IsStarted && _usbChoice.SelectedIndex >= 0;
        }

        private void ShowError(Exception? error)
        {
            ErrorText.Text = "Startup failed: " + (error?.Message ?? "the host did not start.");
            ErrorText.Visibility = Visibility.Visible;
        }

        private static string ReadPackageVersion()
        {
            PackageVersion version = Package.Current.Id.Version;
            return string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}", version.Major, version.Minor, version.Build);
        }
    }
}
