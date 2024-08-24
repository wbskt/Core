using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Wbskt.Core.Web.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly IUsersService _usersService;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthService(IConfiguration configuration, ILogger<AuthService> logger, IUsersService usersService, IPasswordHasher<User> passwordHasher)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        public string GenerateSalt()
        {
            var buffer = new byte[16];
            RandomNumberGenerator.Fill(buffer);

            return Convert.ToBase64String(buffer);
        }

        public string GenerateToken(User userData)
        {
            var tokenHandler = new JsonWebTokenHandler();
            var configurationKey = _configuration["Jwt:Key"];

            var key = Encoding.ASCII.GetBytes(configurationKey!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userData.EmailId.ToString()),
                    new Claim(ClaimTypes.Name, userData.UserName),
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            return tokenHandler.CreateToken(tokenDescriptor);
        }

        public User RegisterNewPlayer(UserRegistrationRequest request)
        {
            string salt = GenerateSalt();
            string saltedPassword = request.Password + salt;
            string hashedPassword = _passwordHasher.HashPassword(null!, saltedPassword);
            var user = new User { EmailId = request.EmailId, PasswordHash = hashedPassword, PasswordSalt = salt, UserName = request.UserName };
            return _usersService.AddUser(user);
        }

        public bool ValidatePasswordWithSalt(LoginRequest loginRequest)
        {
            User user = _usersService.GetUserByEmail(loginRequest.Email);

            var saltedPassword = loginRequest.Password + user.PasswordSalt;
            PasswordVerificationResult result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, saltedPassword);

            if (result != PasswordVerificationResult.Success)
            {                
                return false;
            }

            return true;
        }
    }
}
