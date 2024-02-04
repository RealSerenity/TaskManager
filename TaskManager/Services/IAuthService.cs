namespace TaskManager.Services
{
    public interface IAuthService
    {
        public String Authenticate(string username, string password);
    }
}
