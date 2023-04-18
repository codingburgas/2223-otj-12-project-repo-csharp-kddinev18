using BridgeAPI.BLL.Interfaces;
using BridgeAPI.DTO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace BridgeAPI.BLL.Services.Interfaces
{
    public interface ILocalServerCommunicationService
    {
        public Task<JsonObject> AuthenticateAsync(Guid tokenId, string userName, string passwrod);
        public Task<JsonObject> GetDeviceDataAsync(Guid tokenId, string deviceName, int pagingSize, int skipAmount, string startdate, string endDate);
        public Task<JsonObject> GetDevicesAsync(Guid tokenId, Guid userId);
        public Task<JsonObject> GetRowsCountAsync(Guid tokenId, string deviceName, string startDate, string endStart);
        public Task<JsonObject> SendDataToDeviceAsync(Guid tokenId, string deviceName, string data);

    }
}
