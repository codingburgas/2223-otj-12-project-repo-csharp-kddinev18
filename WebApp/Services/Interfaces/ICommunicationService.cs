namespace WebApp.Services.Interfaces
{
    public interface ICommunicationService
    {
        public Task<string> SendRequestAsync(string endPoint, string parameters, HttpMethod method);
    }
}
