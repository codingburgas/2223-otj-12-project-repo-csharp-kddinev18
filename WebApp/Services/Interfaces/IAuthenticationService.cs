namespace WebApp.Services.Interfaces
{
    public interface IAuthenticationService
    {
        public string LogIn(string userName, string password);
        public string Register(string userName, string email, string password);
    }
}
