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

                if (httpResponse.IsSuccessStatusCode)
                {
                    return await httpResponse.Content.ReadAsStringAsync();
                }
                else if (httpResponse.StatusCode == HttpStatusCode.BadRequest)
                {
                    string errorMessage = await httpResponse.Content.ReadAsStringAsync();
                    throw new ArgumentException(errorMessage);
                }
                else if (httpResponse.StatusCode == HttpStatusCode.InternalServerError)
                {
                    string errorMessage = await httpResponse.Content.ReadAsStringAsync();
                    throw new Exception(errorMessage);
                }
                throw new Exception();

            }
            catch (HttpRequestException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception)
            {
                throw new Exception("General error");
            }
        }
    }
}
