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

        public async Task<JsonObject> Authenticate(Guid TokenId, string userName, string passwrod)
        {
            string response = await _server.LocalServerCommunication
            (
                JsonSerializer.Serialize(new { 
                    TokenId = TokenId,
                    UserName = userName,
                    Password = passwrod
                })
            );
            JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(response);
            if (int.Parse(jObject["StatusCode"].ToString())/100 == 2)
            {
                return jObject;
            }
            else
            {
                throw new Exception("Local server error");
            }
        }

        public Task<JsonObject> GetDeviceDataAsync(string request)
        {
            throw new NotImplementedException();
        }

        public Task<JsonObject> GetDevices(string request)
        {
            throw new NotImplementedException();
        }

        public Task<JsonObject> GetRowsCount(string request)
        {
            throw new NotImplementedException();
        }

        public Task<JsonObject> PostDeviceDataAsync(string request)
        {
            throw new NotImplementedException();
        }

        public Task<JsonObject> SendDataToDevice(string request)
        {
            throw new NotImplementedException();
        }
    }
}
