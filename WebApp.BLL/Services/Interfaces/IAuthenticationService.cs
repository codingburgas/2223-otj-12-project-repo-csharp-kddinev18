using Microsoft.AspNetCore.Http;

namespace WebApp.Services.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<string> LogInAsync(string userName, string password);
        public Task<string> RegisterAsync(string userName, string email, string password, IFormFile image);
        public Task<string> LogInLocalServerAsync(string token, string userName, string password);
        public Task SignOut(string token);
        public Task<string> LocalServerSignOut(string token);
    }
}
