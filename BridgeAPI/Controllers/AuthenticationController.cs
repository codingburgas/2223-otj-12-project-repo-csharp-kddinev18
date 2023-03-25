using BridgeAPI.BLL.Interfaces;
using BridgeAPI.BLL.Services.Interfaces;
using BridgeAPI.DTO;
using BridgeAPI.DTO.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BridgeAPI.Controllers
{
    public class AuthenticationController : Controller
    {
        private IAuthenticationService _authenticationService;
        private ITokenService _tokenService;
        private IEncryptionService _encryptionService;
        public AuthenticationController(IAuthenticationService authenticationService, ITokenService tokenService, IEncryptionService encryptionService)
        {
            _authenticationService = authenticationService;
            _tokenService = tokenService;
            _encryptionService = encryptionService;
        }

        [HttpPost]
        public async Task<string> Login(string request)
        {
            JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(request);
            IResponseDataTransferObject user =
                await _authenticationService.LogInAsync(new UserRequestDataTransferObject()
                {
                    UserName = jObject["UserName"].ToString(),
                    Password = jObject["Password"].ToString()
                });
            _tokenService.GenerateToken(user);
            Tuple<byte[], byte[]> KeyIv = _encryptionService.GetKeyAndIVFromPublicKey(jObject["PublicKey"].ToString());

            return await _encryptionService.Encrypt(request, KeyIv);
        }

        [HttpPost]
        public IActionResult LocalServerLogin(string request)
        {
            return View();
        }

        [HttpPost]
        public async Task<bool> Register(string request)
        {
            JsonObject jObject = JsonSerializer.Deserialize<JsonObject>(request);
            return await _authenticationService.RegisterAsync(new UserRequestDataTransferObject()
                {
                    UserName = jObject["UserName"].ToString(),
                    Password = jObject["Password"].ToString()
                });
        }
    }
}
