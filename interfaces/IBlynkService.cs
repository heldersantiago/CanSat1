using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanSat1.interfaces
{
    public enum DeviceStatus
    {
        Connected,
        Disconnected,
    }

    public interface IBlynkService
    {
        Task UpdateVirtualPinAsync(string virtualPin, float value);
        Task<string> ReadVirtualPinValueAsync(string virtualPin);
        Task<DeviceStatus> GetDeviceStatusAsync();
    }
}
