using CanSat1.interfaces;
using System.Net.Http;
using System.Threading.Tasks;

namespace NexusUtils.BlynkIntegration
{
    public class BlynkService : IBlynkService
    {
        private readonly string _token;

        public BlynkService(string token)
        {
            _token = token;
        }

        /// <summary>
        /// Updates the VirtualPin Value.
        /// </summary>
        /// <param name="virtualPin">The VirtualPin to update.</param>
        /// <param name="value">The Value to set.</param>
        /// <returns></returns>
        public async Task UpdateVirtualPinAsync(string virtualPin, float value)
        {
            try
            {
                using HttpClient httpClient = new();
                await httpClient.GetAsync($"https://ny3.blynk.cloud/external/api/update?token={_token}&{virtualPin}={value}");
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the Value of The VirtualPin.
        /// </summary>
        /// <param name="virtualPin">The VirtualPin to Read.</param>
        /// <returns></returns>
        public async Task<string> ReadVirtualPinValueAsync(string virtualPin)
        {
            using HttpClient httpClient = new();
            var response = await httpClient.GetAsync($"https://ny3.blynk.cloud/external/api/get?token={_token}&{virtualPin}");
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Gets the Device Status.
        /// </summary>
        /// <returns>The Device Status.</returns>
        public async Task<DeviceStatus> GetDeviceStatusAsync()
        {
            using HttpClient httpClient = new();
            var response = await httpClient.GetAsync($"https://ny3.blynk.cloud/external/api/isHardwareConnected?token={_token}");
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            return data == "true" ? DeviceStatus.Connected : DeviceStatus.Disconnected;
        }
    }
}
