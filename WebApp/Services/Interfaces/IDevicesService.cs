using System.Text.Json.Nodes;
using WebApp.DataTransferObjects;

namespace WebApp.Services.Interfaces
{
    public interface IDevicesService
    {
        public Task<DevicesDataTransferObject> GetDeviceDataAsync(string token, string deviceName, int pagingSize, int skipAmount);
        public Task<IEnumerable<string>> GetDevicesAsync(string token);
        public Task<int> GetDeviceRowsCountAsync(string token, string deviceName);
        public Task SendDataToDeviceAsync(string token, string deviceName, string data);
    }
}
