using BridgeAPI.DTO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace BridgeAPI.BLL.Services.Interfaces
{
    public interface ILocalServerCommunicationService
    {
        public Task<JsonObject> GetDevices(string request);
        public Task<JsonObject> SendDataToDevice(string request);
        public Task<JsonObject> GetRowsCount(string request);
        public Task<JsonObject> Authenticate(string request);
        public Task<JsonObject> GetDeviceDataAsync(string request);
        public Task<JsonObject> PostDeviceDataAsync(string request);
    }
}
