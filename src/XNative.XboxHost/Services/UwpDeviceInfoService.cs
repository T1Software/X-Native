using System;
using System.Globalization;
using Windows.ApplicationModel;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;
using XNative.Core.Abstractions;
using XNative.Core.Devices;

namespace XNative.XboxHost.Services
{
    /// <summary>
    /// Reads the device description through the Windows Runtime. This
    /// implementation lives in the host because the module libraries target
    /// .NET Standard and cannot reference the Windows Runtime.
    /// </summary>
    public sealed class UwpDeviceInfoService : IDeviceInfoService
    {
        private DeviceInfo? _cached;

        /// <inheritdoc />
        public DeviceInfo GetDeviceInfo()
        {
            return _cached ??= Detect();
        }

        private static DeviceInfo Detect()
        {
            string deviceFamily = ReadDeviceFamily();
            string osVersion = ReadOperatingSystemVersion();
            string architecture = Package.Current.Id.Architecture.ToString();

            EasClientDeviceInformation information = new EasClientDeviceInformation();
            string manufacturer = Fallback(information.SystemManufacturer);
            string model = Fallback(information.SystemProductName);

            return new DeviceInfo(deviceFamily, osVersion, architecture, manufacturer, model);
        }

        private static string ReadDeviceFamily()
        {
            return Fallback(AnalyticsInfo.VersionInfo.DeviceFamily);
        }

        private static string ReadOperatingSystemVersion()
        {
            string raw = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;

            if (!ulong.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out ulong packed))
            {
                return DeviceInfo.UnknownValue;
            }

            ulong major = (packed & 0xFFFF000000000000UL) >> 48;
            ulong minor = (packed & 0x0000FFFF00000000UL) >> 32;
            ulong build = (packed & 0x00000000FFFF0000UL) >> 16;
            ulong revision = packed & 0x000000000000FFFFUL;

            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}.{1}.{2}.{3}",
                major,
                minor,
                build,
                revision);
        }

        private static string Fallback(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? DeviceInfo.UnknownValue : value;
        }
    }
}
