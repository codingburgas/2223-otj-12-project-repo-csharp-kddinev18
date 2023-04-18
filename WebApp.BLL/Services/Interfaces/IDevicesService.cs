using System.Text.Json.Nodes;
using WebApp.Models;

namespace WebApp.Services.Interfaces
{
    public interface IDevicesService
    {
        public Task<DevicesData> GetDeviceDataAsync(string token, string deviceName, int pagingSize, int skipAmount, DateTime start, DateTime end);
        public Task<IEnumerable<string>> GetDevicesAsync(string token);
        public Task<int> GetDeviceRowsCountAsync(string token, string deviceName, DateTime start, DateTime end);
        public Task SendDataToDeviceAsync(string token, string deviceName, string data);
        public void FormatDates(DevicesData devicesData);
    }
}
