using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using WebApp.Services.Interfaces;

namespace WebApp.Services
{
    public class CommunicationService : ICommunicationService
    {
        public async Task<string> SendRequestAsync(string endPoint, string parameters, HttpMethod method)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("https://localhost:7246/");
                string endpoint = endPoint + "?request=" + parameters;

                HttpRequestMessage httpRequest = new HttpRequestMessage(method, endpoint);
                HttpResponseMessage httpResponse = await httpClient.SendAsync(httpRequest);
                JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(await httpResponse.Content.ReadAsStringAsync());

                if (httpResponse.IsSuccessStatusCode)
                {
                    return jObject["message"].ToString();
                }
                else 
                {
                    throw new Exception(jObject["error"].ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
