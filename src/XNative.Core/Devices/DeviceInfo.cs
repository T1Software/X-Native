using System;

namespace XNative.Core.Devices
{
    /// <summary>
    /// Immutable description of the device the application runs on. The values
    /// are produced by the host that owns the platform APIs; the core only
    /// defines their shape.
    /// </summary>
    public sealed class DeviceInfo
    {
        /// <summary>
        /// Value used whenever the host cannot determine a field.
        /// </summary>
        public const string UnknownValue = "Unknown";

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceInfo"/> class.
        /// </summary>
        /// <param name="deviceFamily">The device family, for example <c>Windows.Xbox</c>.</param>
        /// <param name="operatingSystemVersion">The version of the operating system.</param>
        /// <param name="architecture">The processor architecture of the current process.</param>
        /// <param name="manufacturer">The manufacturer reported by the device.</param>
        /// <param name="model">The model reported by the device.</param>
        public DeviceInfo(
            string deviceFamily,
            string operatingSystemVersion,
            string architecture,
            string manufacturer,
            string model)
        {
            DeviceFamily = deviceFamily ?? throw new ArgumentNullException(nameof(deviceFamily));
            OperatingSystemVersion = operatingSystemVersion ?? throw new ArgumentNullException(nameof(operatingSystemVersion));
            Architecture = architecture ?? throw new ArgumentNullException(nameof(architecture));
            Manufacturer = manufacturer ?? throw new ArgumentNullException(nameof(manufacturer));
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }

        /// <summary>
        /// Device family reported by the host, for example <c>Windows.Xbox</c>
        /// or <c>Windows.Desktop</c>.
        /// </summary>
        public string DeviceFamily { get; }

        /// <summary>
        /// Version of the operating system.
        /// </summary>
        public string OperatingSystemVersion { get; }

        /// <summary>
        /// Processor architecture of the current process.
        /// </summary>
        public string Architecture { get; }

        /// <summary>
        /// Manufacturer reported by the device.
        /// </summary>
        public string Manufacturer { get; }

        /// <summary>
        /// Model reported by the device.
        /// </summary>
        public string Model { get; }

        /// <summary>
        /// Indicates whether the device belongs to the Xbox device family.
        /// </summary>
        public bool IsXbox => DeviceFamily.IndexOf("Xbox", StringComparison.OrdinalIgnoreCase) >= 0;

        /// <summary>
        /// Placeholder used when nothing could be detected.
        /// </summary>
        /// <returns>A <see cref="DeviceInfo"/> with every field unknown.</returns>
        public static DeviceInfo CreateUnknown()
        {
            return new DeviceInfo(UnknownValue, UnknownValue, UnknownValue, UnknownValue, UnknownValue);
        }
    }
}
