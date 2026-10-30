using System;
using System.IO;
using System.Text;
using XNative.Platform.Games;
using Xunit;

namespace XNative.Platform.Tests.Games
{
    public sealed class PeExecutableInspectorTests
    {
        [Theory]
        [InlineData(0x014c, false, PeArchitecture.X86)]
        [InlineData(0x8664, true, PeArchitecture.X64)]
        [InlineData(0x01c0, false, PeArchitecture.Arm)]
        [InlineData(0x01c4, false, PeArchitecture.Arm)]
        [InlineData(0xaa64, true, PeArchitecture.Arm64)]
        [InlineData(0x9999, false, PeArchitecture.Unknown)]
        public void ReportsArchitecture(int machine, bool plus, PeArchitecture expected)
        {
            using var stream = Image((ushort)machine, plus);
            var result = PeExecutableInspector.Inspect(stream);
            Assert.True(result.HasValidHeaders, result.Error);
            Assert.Equal(expected, result.Architecture);
            Assert.Equal((ushort)machine, result.Machine);
            Assert.Equal(plus, result.IsPe32Plus);
            Assert.True(result.IsExecutableImage);
            Assert.False(result.IsDll);
        }

        [Fact]
        public void ReportsDllWithoutClaimingItCanLaunch()
        {
            using var stream = Image(flags: 0x2002);
            Assert.True(PeExecutableInspector.Inspect(stream).IsDll);
        }

        [Fact]
        public void ReportsMissingExecutableCharacteristic()
        {
            using var stream = Image(flags: 0);
            var result = PeExecutableInspector.Inspect(stream);
            Assert.True(result.HasValidHeaders);
            Assert.False(result.IsExecutableImage);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(63)]
        [InlineData(int.MaxValue)]
        public void RejectsInvalidPeOffsets(int offset)
        {
            using var stream = Image();
            Write32(stream, 0x3c, unchecked((uint)offset));
            Assert.False(PeExecutableInspector.Inspect(stream).HasValidHeaders);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(63)]
        [InlineData(150)]
        [InlineData(200)]
        [InlineData(415)]
        public void RejectsTruncatedHeaders(int length)
        {
            using var stream = Image();
            stream.SetLength(length);
            Assert.False(PeExecutableInspector.Inspect(stream).HasValidHeaders);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(128, 0)]
        [InlineData(152, 0x107)]
        [InlineData(134, 0)]
        [InlineData(134, 97)]
        [InlineData(148, 2)]
        public void RejectsMalformedHeaders(int offset, int value)
        {
            using var stream = Image();
            Write16(stream, offset, (ushort)value);
            Assert.False(PeExecutableInspector.Inspect(stream).HasValidHeaders);
        }

        [Fact]
        public void RejectsOversizedDataDirectoryCount()
        {
            using var stream = Image();
            Write32(stream, 244, uint.MaxValue);
            Assert.False(PeExecutableInspector.Inspect(stream).HasValidHeaders);
        }

        [Theory]
        [InlineData(0x014c, true)]
        [InlineData(0x8664, false)]
        [InlineData(0xaa64, false)]
        public void RejectsMachineFormatMismatch(int machine, bool plus)
        {
            using var stream = Image((ushort)machine, plus);
            Assert.False(PeExecutableInspector.Inspect(stream).HasValidHeaders);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void RestoresPositionAndLeavesStreamOpen(bool invalid)
        {
            using var stream = Image();
            if (invalid) Write16(stream, 0, 0);
            stream.Position = 17;
            var result = PeExecutableInspector.Inspect(stream);
            Assert.True(result.HasValidHeaders != invalid);
            Assert.Equal(17L, stream.Position);
            Assert.True(stream.CanRead);
        }

        [Fact]
        public void RejectsNullStream()
        {
            Assert.Throws<ArgumentNullException>(() => PeExecutableInspector.Inspect(null!));
        }

        private static MemoryStream Image(ushort machine = 0x014c, bool plus = false, ushort flags = 2)
        {
            var stream = new MemoryStream();
            stream.SetLength(512);
            using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);
            writer.Write((ushort)0x5a4d);
            stream.Position = 0x3c;
            writer.Write(128);
            stream.Position = 128;
            writer.Write(0x00004550u);
            writer.Write(machine);
            writer.Write((ushort)1);
            stream.Position = 148;
            writer.Write((ushort)(plus ? 240 : 224));
            writer.Write(flags);
            writer.Write((ushort)(plus ? 0x20b : 0x10b));
            stream.Position = 0;
            return stream;
        }

        private static void Write16(Stream stream, int offset, ushort value)
        {
            stream.Position = offset;
            using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);
            writer.Write(value);
        }

        private static void Write32(Stream stream, int offset, uint value)
        {
            stream.Position = offset;
            using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);
            writer.Write(value);
        }
    }
}
