using BridgeAPI.BLL.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BridgeAPI.BLL.Services
{
    public class ResponseFormatterService : IResponseFormatterService
    {
        public string FormatResponse(int statusCode, string response, string errorMessage, Dictionary<string, string> additionalInformation)
        {
            return JsonSerializer.Serialize(new
            {
                StatusCode = statusCode,
                Response = response,
                errorMessage = errorMessage ?? null,
                AdditionalInformation = additionalInformation
            },
            new JsonSerializerOptions
            {
                IgnoreNullValues = true
            });
        }
    }
}
