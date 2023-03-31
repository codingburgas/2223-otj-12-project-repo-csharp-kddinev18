using WebApp.Services.Interfaces;

namespace WebApp.Services
{
    public class CommunicationService : ICommunicationService
    {
        public async Task<string> SendRequestAsync(string endPoint, string parameters, HttpMethod method)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7246/");
            string endpoint = endPoint + "?request=" + parameters;

            HttpRequestMessage request = new HttpRequestMessage(method, endpoint);
            HttpResponseMessage response = await client.SendAsync(request);

            return await response.Content.ReadAsStringAsync();
        }
    }
}
