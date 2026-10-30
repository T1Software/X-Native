using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XNative.Core.Abstractions;

namespace XNative.Platform.Storage
{
    /// <summary>File access within the permissions of the current host; grants no sandbox bypass.</summary>
    public sealed class PhysicalFileSystemService : IFileSystemService
    {
        public bool DirectoryExists(string path) => Directory.Exists(path);
        public bool FileExists(string path) => File.Exists(path);
        public string[] GetFiles(string directory, string pattern) =>
            Directory.GetFiles(directory, pattern, SearchOption.TopDirectoryOnly);
        public string[] GetDirectories(string directory) =>
            Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly);

        /// <summary>
        /// Reads UTF-8 text with BOM detection. Cancellation is checked before opening
        /// and after reading; .NET Standard 2.0 ReadToEndAsync has no token overload.
        /// I/O and access errors propagate to the catalogue or host.
        /// </summary>
        public async Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read,
                FileShare.Read, 4096, useAsync: true);
            using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            var text = await reader.ReadToEndAsync().ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            return text;
        }
    }
}
