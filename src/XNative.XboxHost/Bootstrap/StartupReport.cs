using System;

namespace XNative.XboxHost.Bootstrap
{
    /// <summary>
    /// Result of the startup sequence of the host.
    /// </summary>
    public sealed class StartupReport
    {
        private StartupReport(bool isSuccess, IServiceProvider? services, Exception? error)
        {
            IsSuccess = isSuccess;
            Services = services;
            Error = error;
        }

        /// <summary>
        /// Indicates whether the container was built and validated.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// The service provider, when the startup succeeded.
        /// </summary>
        public IServiceProvider? Services { get; }

        /// <summary>
        /// The failure, when the startup failed.
        /// </summary>
        public Exception? Error { get; }

        /// <summary>
        /// Creates a successful report.
        /// </summary>
        /// <param name="services">The service provider that was built.</param>
        /// <returns>The report.</returns>
        public static StartupReport Succeeded(IServiceProvider services)
        {
            return new StartupReport(true, services, null);
        }

        /// <summary>
        /// Creates a failed report.
        /// </summary>
        /// <param name="error">The failure that occurred.</param>
        /// <returns>The report.</returns>
        public static StartupReport Failed(Exception error)
        {
            return new StartupReport(false, null, error);
        }
    }
}
