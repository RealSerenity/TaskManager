namespace TaskManager.Services.Impl
{
    public class AuthServiceImpl : IAuthService
    {
      private readonly IUserService _userService;
        private readonly IPasswordHasher  _passwordHasher;
        private readonly IJwtService _jwtService;
        public AuthServiceImpl(IUserService userService, IPasswordHasher passwordHasher, IJwtService jwtService)
        {
            _userService = userService;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }
        public String Authenticate(string username, string password)
        {
            // Kullanıcı adı ve şifreyle kullanıcıyı veritabanından arayarak kontrol et
            var user = _userService.GetByUsername(username);

            // Kullanıcı bulunamazsa veya şifre doğrulanamazsa, doğrulama başarısız olur
            if (user == null || !_passwordHasher.VerifyPassword(password, user.Password))
                return "User not found or wrong credentials";

            // Kullanıcı doğrulandı
            return _jwtService.GenerateJwtToken(user.Id.ToString(), user.Username);
        }
    }
}
