using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeAPI.BLL.Services.Interfaces
{
    public interface IResponseFormatterService
    {
        public IActionResult FormatResponse(int statusCode, string response, string errorMessage, Dictionary<string, string> additionalInformation);
        public string FormatResponseToString(int statusCode, string response, string errorMessage, Dictionary<string, string> additionalInformation);
    }
}
