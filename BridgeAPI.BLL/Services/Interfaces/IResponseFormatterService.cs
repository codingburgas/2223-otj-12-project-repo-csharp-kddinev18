using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeAPI.BLL.Services.Interfaces
{
    public interface IResponseFormatterService
    {
        public string FormatResponse(int statusCode, string response, string errorMessage, Dictionary<string, string> additionalInformation);
    }
}
