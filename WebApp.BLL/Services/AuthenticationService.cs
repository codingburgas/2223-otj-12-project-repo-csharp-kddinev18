using System.Text.Json;
using WebApp.Services.Interfaces;

namespace WebApp.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private ICommunicationService _communicationService;
        public AuthenticationService(ICommunicationService communicationService)
        {
            _communicationService = communicationService;
        }
        public async Task<string> LogInAsync(string userName, string password)
        {
            return await _communicationService.SendRequestAsync(
                "Authentication/LogIn",
                JsonSerializer.Serialize(
                    new
                    {
                        UserName = userName,
                        Password = password
                    }
                ),
                HttpMethod.Get
            );
        }

        public async Task<string> RegisterAsync(string userName, string email, string password)
        {
            return await _communicationService.SendRequestAsync(
                "Authentication/Register",
                JsonSerializer.Serialize(
                    new {
                        UserName = userName,
                        Email = email,
                        Password = password
                    }
                ),
                HttpMethod.Get
            );
        }

        public async Task<string> LogInLocalServerAsync(string token, string userName, string password)
        {
            return await _communicationService.SendRequestAsync(
                "Authentication/LocalServerLogIn",
                JsonSerializer.Serialize(
                    new
                    {
                        Token = token,
                        Arguments = new
                        {
                            UserName = userName,
                            Password = password
                        }
                    }
                ),
                HttpMethod.Get
            );
        }

        public async Task SignOut(string token)
        {
            await _communicationService.SendRequestAsync(
                "Authentication/SignOut",
                JsonSerializer.Serialize(
                    new
                    {
                        Token = token,
                    }
                ),
                HttpMethod.Get
            );
        }

        public async Task<string> LocalServerSignOut(string token)
        {
            return await _communicationService.SendRequestAsync(
                "Authentication/LocalServerSignOut",
                JsonSerializer.Serialize(
                    new
                    {
                        Token = token,
                    }
                ),
                HttpMethod.Get
            );
        }
    }
}
