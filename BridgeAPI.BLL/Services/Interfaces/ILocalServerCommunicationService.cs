using BridgeAPI.DTO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeAPI.BLL.Services.Interfaces
{
    public interface ILocalServerCommunicationService
    {
        public Task<IResponseDataTransferObject> LogInAsync(string message);
        public Task<string> GetDeviceDataAsync(string message);
        public Task<bool> PostDeviceDataAsync(string message);
    }
}
