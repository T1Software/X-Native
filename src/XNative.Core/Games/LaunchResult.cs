using System;

namespace XNative.Core.Games
{
    /// <summary>
    /// Result of a game launch operation.
    /// </summary>
    public sealed class LaunchResult
    {
        private LaunchResult(bool success, string? message, int? processId)
        {
            Success = success;
            Message = message;
            ProcessId = processId;
        }

        /// <summary>
        /// Indicates whether the launch was successful.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// A message describing the result.
        /// </summary>
        public string? Message { get; }

        /// <summary>
        /// The process ID if the launch succeeded, otherwise null.
        /// </summary>
        public int? ProcessId { get; }

        /// <summary>
        /// Creates a successful launch result.
        /// </summary>
        /// <param name="message">Optional message.</param>
        /// <param name="processId">Optional process ID.</param>
        /// <returns>A successful <see cref="LaunchResult"/>.</returns>
        public static LaunchResult Ok(string? message = null, int? processId = null)
        {
            return new LaunchResult(true, message, processId);
        }

        /// <summary>
        /// Creates a failed launch result.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>A failed <see cref="LaunchResult"/>.</returns>
        public static LaunchResult Fail(string message)
        {
            return new LaunchResult(false, message, null);
        }
    }
}