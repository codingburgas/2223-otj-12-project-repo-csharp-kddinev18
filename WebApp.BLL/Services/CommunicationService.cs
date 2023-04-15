using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using WebApp.Services.Interfaces;

namespace WebApp.Services
{
    public class CommunicationService : ICommunicationService
    {
        private IHttpClientFactory _httpClientFactory;
        public CommunicationService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> SendRequestAsync(string endPoint, string parameters, HttpMethod method)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7246/");

                HttpContent httpContent = new StringContent(parameters, Encoding.UTF8, "application/json");

                HttpRequestMessage httpRequest = new HttpRequestMessage(method, endPoint);
                httpRequest.Content = httpContent;

                HttpResponseMessage httpResponse = await client.SendAsync(httpRequest);

                string content = await httpResponse.Content.ReadAsStringAsync();
                JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(content);

                if (httpResponse.IsSuccessStatusCode)
                {
                    return jObject["message"].ToString();
                }
                else if(httpResponse.StatusCode == (HttpStatusCode)401)
                {
                    throw new UnauthorizedAccessException(jObject["error"].ToString());
                }
                else
                {
                    throw new Exception(jObject["error"].ToString());
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
