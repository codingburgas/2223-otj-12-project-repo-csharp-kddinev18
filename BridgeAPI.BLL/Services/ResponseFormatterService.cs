using BridgeAPI.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BridgeAPI.BLL.Services
{
    public class ResponseFormatterService : IResponseFormatterService
    {
        public IActionResult FormatResponse(int statusCode, string response, string errorMessage, Dictionary<string, string> additionalInformation)
        {
            var responseObject = new
            {
                status = statusCode,
                message = response,
                error = errorMessage,
                additional_info = additionalInformation
            };

            return new JsonResult(responseObject) { StatusCode = statusCode };
        }

        public string FormatResponseToString(int statusCode, string response, string errorMessage, Dictionary<string, string> additionalInformation)
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
