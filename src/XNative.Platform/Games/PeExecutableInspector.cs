using System;
using System.IO;
using System.Text;

namespace XNative.Platform.Games
{
    public enum PeArchitecture { Unknown, X86, X64, Arm, Arm64 }

    /// <summary>Header inspection only; never a promise that an image can execute.</summary>
    public sealed class PeInspectionResult
    {
        public bool HasValidHeaders { get; }
        public PeArchitecture Architecture { get; }
        public ushort Machine { get; }
        public bool IsPe32Plus { get; }
        public bool IsDll { get; }
        public bool IsExecutableImage { get; }
        public string? Error { get; }

        internal PeInspectionResult(string error) { Error = error; }
        internal PeInspectionResult(ushort machine, bool plus, ushort flags)
        {
            HasValidHeaders = true;
            Machine = machine;
            Architecture = machine switch
            {
                0x014c => PeArchitecture.X86,
                0x8664 => PeArchitecture.X64,
                0x01c0 or 0x01c4 => PeArchitecture.Arm,
                0xaa64 => PeArchitecture.Arm64,
                _ => PeArchitecture.Unknown
            };
            IsPe32Plus = plus;
            IsDll = (flags & 0x2000) != 0;
            IsExecutableImage = (flags & 0x0002) != 0;
        }
    }

    /// <summary>Inspects bounded PE headers without loading code or allocating from file sizes.</summary>
    public static class PeExecutableInspector
    {
        /// <summary>
        /// Reads from offset zero and restores the caller's position. Leaves the stream open.
        /// Requires a readable, seekable stream. I/O errors propagate to the caller.
        /// Does not validate section contents, imports, signatures or runtime compatibility.
        /// </summary>
        public static PeInspectionResult Inspect(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead || !stream.CanSeek)
                throw new ArgumentException("A readable, seekable stream is required.", nameof(stream));

            var originalPosition = stream.Position;
            try
            {
                var length = stream.Length;
                if (length < 64) return Invalid("Truncated DOS header.");
                stream.Position = 0;
                using var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
                if (reader.ReadUInt16() != 0x5a4d) return Invalid("Missing MZ signature.");
                stream.Position = 0x3c;
                var peOffset = reader.ReadInt32();
                if (peOffset < 64 || (long)peOffset > length - 24)
                    return Invalid("PE header offset is outside the file.");
                stream.Position = peOffset;
                if (reader.ReadUInt32() != 0x00004550) return Invalid("Missing PE signature.");
                var machine = reader.ReadUInt16();
                var sections = reader.ReadUInt16();
                stream.Position += 12;
                var optionalSize = reader.ReadUInt16();
                var flags = reader.ReadUInt16();
                var optionalStart = stream.Position;
                if (sections == 0 || sections > 96) return Invalid("Invalid section count.");
                if (optionalSize < 2 || optionalStart + optionalSize > length)
                    return Invalid("Truncated optional header.");
                var magic = reader.ReadUInt16();
                if (magic != 0x010b && magic != 0x020b)
                    return Invalid("Unsupported optional header magic.");
                var plus = magic == 0x020b;
                var fixedSize = plus ? 112 : 96;
                if (optionalSize < fixedSize) return Invalid("Optional header is too small.");
                if (((machine == 0x014c || machine == 0x01c0 || machine == 0x01c4) && plus) ||
                    ((machine == 0x8664 || machine == 0xaa64) && !plus))
                    return Invalid("Machine and optional header format disagree.");
                stream.Position = optionalStart + fixedSize - 4;
                var directoryCount = reader.ReadUInt32();
                if (directoryCount > (optionalSize - fixedSize) / 8)
                    return Invalid("Data directories exceed the optional header.");
                if (optionalStart + optionalSize + (long)sections * 40 > length)
                    return Invalid("Truncated section table.");
                return new PeInspectionResult(machine, plus, flags);
            }
            catch (EndOfStreamException)
            {
                return Invalid("Image was truncated while reading headers.");
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }

        private static PeInspectionResult Invalid(string error) => new PeInspectionResult(error);
    }
}
