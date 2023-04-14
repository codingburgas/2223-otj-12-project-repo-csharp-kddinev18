using System.Text.Json.Nodes;
using WebApp.Models;

namespace WebApp.Services.Interfaces
{
    public interface IDevicesService
    {
        public Task<DevicesData> GetDeviceDataAsync(string token, string deviceName, int pagingSize, int skipAmount);
        public Task<IEnumerable<string>> GetDevicesAsync(string token);
        public Task<int> GetDeviceRowsCountAsync(string token, string deviceName);
        public Task SendDataToDeviceAsync(string token, string deviceName, string data);
    }
}
