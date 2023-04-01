namespace WebApp.Services.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<string> LogInAsync(string userName, string password);
        public Task<string> RegisterAsync(string userName, string email, string password);
        public Task<string> LogInLocalServerAsync(string token, string userName, string password);
    }
}
