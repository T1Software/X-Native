using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;

namespace XNative.Platform.Games
{
    /// <summary>Extracts a USB game ZIP into a new private staging directory.</summary>
    public static class GameArchiveImporter
    {
        public const long DefaultMaxBytes = 4L * 1024 * 1024 * 1024;
        public const int DefaultMaxEntries = 10000;

        /// <summary>Leaves input open. Removes partial output on failure. Never overwrites a destination.</summary>
        public static int Extract(Stream input, string destination, CancellationToken token = default,
            long maxBytes = DefaultMaxBytes, int maxEntries = DefaultMaxEntries)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (maxBytes <= 0 || maxEntries <= 0) throw new ArgumentOutOfRangeException(nameof(maxBytes));
            token.ThrowIfCancellationRequested();
            var root = Path.GetFullPath(destination);
            if (Directory.Exists(root) || File.Exists(root)) throw new IOException("Import destination already exists.");
            Directory.CreateDirectory(root);
            try
            {
                using var archive = new ZipArchive(input, ZipArchiveMode.Read, leaveOpen: true);
                if (archive.Entries.Count > maxEntries) throw new InvalidDataException("Archive has too many entries.");
                var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var buffer = new byte[65536];
                long total = 0;
                int files = 0;
                foreach (var entry in archive.Entries)
                {
                    token.ThrowIfCancellationRequested();
                    var path = entry.FullName.Replace('\\', '/');
                    var parts = path.TrimEnd('/').Split('/');
                    if (path.StartsWith("/", StringComparison.Ordinal) || parts.Length > 32 ||
                        parts.Any(p => string.IsNullOrEmpty(p) || p == "." || p == ".." ||
                            p.EndsWith(".", StringComparison.Ordinal) || p.EndsWith(" ", StringComparison.Ordinal) ||
                            IsDeviceName(p) || p.Any(c => c < 32 || "<>:\"|?*".IndexOf(c) >= 0)))
                        throw new InvalidDataException("Unsafe archive path: " + entry.FullName);
                    if ((((uint)entry.ExternalAttributes >> 16) & 0xf000) == 0xa000 ||
                        (entry.ExternalAttributes & (int)FileAttributes.ReparsePoint) != 0)
                        throw new InvalidDataException("Archive links are not supported.");
                    var target = Path.Combine(root, Path.Combine(parts));
                    if (path.EndsWith("/", StringComparison.Ordinal))
                    {
                        Directory.CreateDirectory(target);
                        continue;
                    }
                    if (!names.Add(target)) throw new InvalidDataException("Duplicate archive path.");
                    if (entry.Length > maxBytes - total) throw new InvalidDataException("Archive exceeds the extraction limit.");
                    Directory.CreateDirectory(Path.GetDirectoryName(target)!);
                    using var source = entry.Open();
                    using var output = new FileStream(target, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                    long written = 0;
                    int count;
                    while ((count = source.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        token.ThrowIfCancellationRequested();
                        if (count > maxBytes - total) throw new InvalidDataException("Archive exceeds the extraction limit.");
                        output.Write(buffer, 0, count);
                        total += count;
                        written += count;
                    }
                    if (written != entry.Length) throw new InvalidDataException("Archive entry length mismatch.");
                    files++;
                }
                return files;
            }
            catch
            {
                try { Directory.Delete(root, recursive: true); }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
                throw;
            }
        }
        private static bool IsDeviceName(string name)
        {
            var stem = name.Split('.')[0].ToUpperInvariant();
            return stem == "CON" || stem == "PRN" || stem == "AUX" || stem == "NUL" ||
                (stem.Length == 4 && (stem.StartsWith("COM", StringComparison.Ordinal) ||
                stem.StartsWith("LPT", StringComparison.Ordinal)) && stem[3] >= '1' && stem[3] <= '9');
        }
    }
}
