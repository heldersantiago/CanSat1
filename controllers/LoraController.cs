using CanSat1.interfaces;
using CanSat1.models;
using CanSat1.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanSat1.controllers
{
    public class LoraController
    {
        private readonly ILoraService _loraService;
        private readonly IBlynkService _blynkService;
        public LoraController(ILoraService loraService, IBlynkService blynkService) 
        {
            _loraService = loraService;
            _blynkService = blynkService;
            _loraService.DataReceived += LoraService_DataReceived;
        }

        private async void LoraService_DataReceived(string dataReceived)
        {
            if (IsValidJson(dataReceived))
            {
                var sensors = JsonConvert.DeserializeObject<Sensor>(dataReceived);
                await _blynkService.UpdateVirtualPinAsync("v1", float.Parse(sensors.Temperature));
                await _blynkService.UpdateVirtualPinAsync("v2", float.Parse(sensors.Humidity));
            }
        }

        public bool Connect(string port, int baudRate)
        {
            _loraService.Port = port;
            _loraService.BaudRate = baudRate;
            return _loraService.Connect();
        }

        public void Disconnect()
        {
            _loraService.Disconnect();
        }

        private bool IsValidJson(string str)
        {
            try
            {
                JToken.Parse(str);
                return true;
            }
            catch (JsonReaderException)
            {
                return false;
            }
        }
    }
}
