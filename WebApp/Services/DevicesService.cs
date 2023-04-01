using System.Collections;
using System.Text.Json;
using System.Text.Json.Nodes;
using WebApp.DataTransferObjects;
using WebApp.Services.Interfaces;

namespace WebApp.Services
{
    public class DevicesService : IDevicesService
    {
        private ICommunicationService _communicationService;
        public DevicesService(ICommunicationService communicationService)
        {
            _communicationService = communicationService;
        }

        public async Task<DevicesDataTransferObject> GetDevices(string token)
        {
            string response = await _communicationService.SendRequestAsync(
                "DeviceData/GetDevices",
                JsonSerializer.Serialize(
                    new
                    {
                        Token = token,
                    }
                ),
                HttpMethod.Get
            );
            JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(response);
            return new DevicesDataTransferObject()
            {
                DeviceNames = JsonSerializer.Deserialize<IEnumerable<string>>(jObject["Devices"].ToString())
            };
        }

        public async Task<int> GetDeviceRows(string token, string deviceName)
        {
            string response = await _communicationService.SendRequestAsync(
                "DeviceData/GetDeviceData",
                JsonSerializer.Serialize(
                    new { 
                        Token = token, 
                        Arguemnts = new { DeviceName = deviceName } 
                    }
                ),
                HttpMethod.Get
            );
            JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(response);
            return int.Parse(jObject["Count"].ToString());
        }
    }
}
