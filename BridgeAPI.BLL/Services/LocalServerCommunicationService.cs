using BridgeAPI.BLL.Interfaces;
using BridgeAPI.BLL.Services.Interfaces;
using BridgeAPI.DTO;
using BridgeAPI.DTO.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace BridgeAPI.BLL.Services
{
    public class LocalServerCommunicationService : ILocalServerCommunicationService
    {
        private IServer _server;

        public LocalServerCommunicationService(IServer server)
        {
            _server = server;
        }

        public async Task<string> GetDeviceDataAsync(string message)
        {
            return await Task.Run(() => _server.LocalServerCommunication(message));
        }

        public async Task<IResponseDataTransferObject> LogInAsync(string message)
        {
            string response = await Task.Run(()=>_server.LocalServerCommunication(message));
            JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(response);

            return new UserResponseDataTransferObject()
            {
                LocalServerId = new Guid(jObject["Id"].ToString()),
                UserName = jObject["UserName"].ToString(),
                Email = jObject["Email"].ToString()
            };
        }

        public async Task<bool> PostDeviceDataAsync(string message)
        {
            string response = await Task.Run(() => _server.LocalServerCommunication(message));
            JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(response);
            if (jObject["Status"].ToString() == "Success")
            {
                return true;
            }
            return false;
        }
    }
}
