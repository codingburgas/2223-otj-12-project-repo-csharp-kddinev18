using BridgeAPI.BLL.Interfaces;
using BridgeAPI.BLL.Services.Interfaces;
using BridgeAPI.DAL.Models;
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
        public async Task<JsonObject> AuthenticateAsync(Guid tokenId, string userName, string passwrod)
        {
            string response = await Server.LocalServerCommunication
            (
                JsonSerializer.Serialize(new
                {
                    TokenId = tokenId,
                    OperationType = "Authenticate",
                    Arguments = new
                    {
                        UserName = userName,
                        Password = passwrod
                    }
                })
            );
            return ValidateResponse(response);
        }

        public async Task<JsonObject> GetDeviceDataAsync(Guid tokenId, string deviceName, int pagingSize, int skipAmount)
        {
            string response = await Server.LocalServerCommunication
            (
                JsonSerializer.Serialize(new
                {
                    TokenId = tokenId,
                    OperationType = "GetDeviceData",
                    Arguments = new
                    {
                        DeviceName = deviceName,
                        PagingSize = pagingSize,
                        SkipAmount = skipAmount
                    }
                })
            );
            return ValidateResponse(response);
        }

        public async Task<JsonObject> GetDevicesAsync(Guid tokenId, Guid userId)
        {
            string response = await Server.LocalServerCommunication
            (
                JsonSerializer.Serialize(new
                {
                    TokenId = tokenId,
                    OperationType = "GetDevices",
                    Arguments = new
                    {
                        UserId = userId,
                    }
                })
            );
            return ValidateResponse(response);
        }

        public async Task<JsonObject> GetRowsCountAsync(Guid tokenId, string deviceName)
        {
            string response = await Server.LocalServerCommunication
            (
                JsonSerializer.Serialize(new
                {
                    TokenId = tokenId,
                    OperationType = "GetRowsCount",
                    Arguments = new
                    {
                        DeviceName = deviceName,
                    }
                })
            );
            return ValidateResponse(response);
        }

        public async Task<JsonObject> SendDataToDeviceAsync(Guid tokenId, string deviceName, string data)
        {
            string response = await Server.LocalServerCommunication
            (
                JsonSerializer.Serialize(new
                {
                    TokenId = tokenId,
                    OperationType = "SendDataToDevice",
                    Arguments = new
                    {
                        DeviceName = deviceName,
                        Data = data,
                    }
                })
            );
            return ValidateResponse(response);
        }


        private JsonObject ValidateResponse(string response)
        {
            JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(response);
            if (int.Parse(jObject["StatusCode"].ToString()) / 100 == 2)
            {
                return JsonSerializer.Deserialize<JsonObject>(jObject["Response"].ToString());
            }
            else
            {
                throw new Exception("Local server error");
            }
        }
    }
}
