using XNative.Core.Devices;

namespace XNative.Core.Abstractions
{
    /// <summary>
    /// Describes the device the application currently runs on, including the
    /// capabilities that the compatibility modules must adapt to.
    /// Implemented by the host that owns the platform APIs.
    /// </summary>
    public interface IDeviceInfoService
    {
        /// <summary>
        /// Returns the description of the current device.
        /// </summary>
        /// <returns>The current device description.</returns>
        DeviceInfo GetDeviceInfo();
    }
}
