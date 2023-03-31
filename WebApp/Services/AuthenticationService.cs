using Microsoft.AspNetCore.Mvc;
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


    }
}
