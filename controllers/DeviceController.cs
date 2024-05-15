using CanSat1.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanSat1.controllers
{
    public class DeviceController
    {
        private readonly IBlynkService _blynkService;
        public DeviceController(IBlynkService blynkService) {
            _blynkService = blynkService;
        }

        public async Task UpdateDevicePinAsync(string pin, float value)
        {
            await _blynkService.UpdateVirtualPinAsync(pin, value);
        }

        public async Task<string> GetDevicePinValueAsync(string pin)
        {
            return await _blynkService.ReadVirtualPinValueAsync(pin);
        }

        public async Task<DeviceStatus> CheckDeviceStatusAsync()
        {
            return await _blynkService.GetDeviceStatusAsync();
        }
    }   
}
