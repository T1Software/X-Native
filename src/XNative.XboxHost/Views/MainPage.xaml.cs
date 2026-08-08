using System;
using System.Globalization;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using XNative.UI.ViewModels;
using XNative.XboxHost.Bootstrap;

namespace XNative.XboxHost.Views
{
    /// <summary>
    /// Startup screen. It reports the modules resolved by the container and
    /// the device the application runs on, plus a game library scanner.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            VersionText.Text = "Version " + ReadPackageVersion();
        }

        /// <summary>
        /// Startup view model backing the page.
        /// </summary>
        public StartupViewModel? ViewModel { get; private set; }

        /// <summary>
        /// Game library view model.
        /// </summary>
        public GameLibraryViewModel? GameLibrary { get; private set; }

        /// <inheritdoc />
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            StartupReport? report = App.Startup;

            if (report == null || !report.IsSuccess)
            {
                ShowError(report?.Error);
                return;
            }

            StartupViewModel viewModel = Bootstrapper.Resolve<StartupViewModel>();
            viewModel.Load();

            ViewModel = viewModel;

            GameLibrary = Bootstrapper.Resolve<GameLibraryViewModel>();

            Bindings.Update();
        }

        private void ShowError(Exception? error)
        {
            ErrorText.Text = "Startup failed: " + (error?.Message ?? "the host did not start.");
            ErrorText.Visibility = Visibility.Visible;
        }

        private static string ReadPackageVersion()
        {
            PackageVersion version = Package.Current.Id.Version;

            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}.{1}.{2}",
                version.Major,
                version.Minor,
                version.Build);
        }
    }
}