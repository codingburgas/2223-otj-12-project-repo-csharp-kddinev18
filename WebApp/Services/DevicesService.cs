﻿using System.Collections;
using System.Text.Json;
using System.Text.Json.Nodes;
using WebApp.Models;
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

        public async Task<DevicesData> GetDeviceDataAsync(string token, string deviceName, int pagingSize, int skipAmount)
        {
            string response = await _communicationService.SendRequestAsync(
                "DeviceData/GetDeviceData",
                JsonSerializer.Serialize(
                    new
                    {
                        Token = token,
                        Arguments = new
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
            DevicesData devicesData = new DevicesData()
            {
                Infrastructure = JsonSerializer.Deserialize<IEnumerable<string>>(jObject["Infrastructure"].ToString()),
                Data = JsonSerializer.Deserialize<IEnumerable<JsonObject>>(jObject["Data"].ToString()),
                Name = deviceName
            };

            foreach (JsonObject item in devicesData.Data)
            {
                item["Created"] = DateTime.ParseExact(item["Created"].ToString(), "yyyy-MM-dd'T'HH:mm:ss", null).ToString("HH:mm:ss");
            }
        }

        public async Task<IEnumerable<string>> GetDevicesAsync(string token)
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
            return JsonSerializer.Deserialize<IEnumerable<string>>(jObject["Devices"].ToString());
        }

        public async Task<int> GetDeviceRowsCountAsync(string token, string deviceName)
        {
            string response = await _communicationService.SendRequestAsync(
                "DeviceData/GetRowsCount",
                JsonSerializer.Serialize(
                    new
                    {
                        Token = token,
                        Arguments = new { DeviceName = deviceName }
                    }
                ),
                HttpMethod.Get
            );
            JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(response);
            return int.Parse(jObject["Count"].ToString());
        }

        public async Task SendDataToDeviceAsync(string token, string deviceName, string data)
        {
            await _communicationService.SendRequestAsync(
                "DeviceData/SendDataToDevice",
                JsonSerializer.Serialize(
                    new
                    {
                        Token = token,
                        Arguments = new { DeviceName = deviceName, Data = data }
                    }
                ),
                HttpMethod.Get
            );
        }
    }
}
