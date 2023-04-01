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

<<<<<<< HEAD
        public async Task<DeviceDataDataTransferObject> GetDeviceDataAsync(string token, string deviceName, int pagingSize, int skipAmount)
        {
            string response = await _communicationService.SendRequestAsync(
                "DeviceData/GetDeviceData",
                JsonSerializer.Serialize(
                    new
                    {
                        Token = token,
                        Arguemnts = new 
                        { 
                            DeviceName = deviceName,
                            PagingSize = pagingSize,
                            SkipAmount = skipAmount
                        }
                    }
                ),
                HttpMethod.Get
            );
            JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(response);
            return new DeviceDataDataTransferObject()
            {
                Infrastructure = JsonSerializer.Deserialize<IEnumerable<string>>(jObject["Infrastructure"].ToString()),
                Data = JsonSerializer.Deserialize<IEnumerable<JsonObject>>(jObject["Data"].ToString())
            };
        }

        public async Task<DevicesDataTransferObject> GetDevicesAsync(string token)
=======
        public async Task<DevicesDataTransferObject> GetDevices(string token)
>>>>>>> 63e0e0ccc07df846e2798ad5641a2ec1a281a942
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

<<<<<<< HEAD
        public async Task<int> GetDeviceRowsAsync(string token, string deviceName)
=======
        public async Task<int> GetDeviceRows(string token, string deviceName)
>>>>>>> 63e0e0ccc07df846e2798ad5641a2ec1a281a942
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
<<<<<<< HEAD

        public async Task SendDataToDeviceAsync(string token, string deviceName, string data)
        {
            await _communicationService.SendRequestAsync(
                "DeviceData/SendDataToDevice",
                JsonSerializer.Serialize(
                    new
                    {
                        Token = token,
                        Arguemnts = new { DeviceName = deviceName, Data = data }
                    }
                ),
                HttpMethod.Get
            );
        }
=======
>>>>>>> 63e0e0ccc07df846e2798ad5641a2ec1a281a942
    }
}