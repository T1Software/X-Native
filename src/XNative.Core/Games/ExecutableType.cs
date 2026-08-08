namespace XNative.Core.Games
{
    /// <summary>
    /// Type of executable that can be launched.
    /// </summary>
    public enum ExecutableType
    {
        /// <summary>
        /// Unknown or undetermined executable type.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// A traditional Windows .exe file.
        /// </summary>
        WindowsExecutable = 1,

        /// <summary>
        /// A UWP application.
        /// </summary>
        UwpApplication = 2,
    }
}