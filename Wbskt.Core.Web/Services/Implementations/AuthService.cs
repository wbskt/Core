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
        private readonly IConfiguration configuration;
        private readonly ILogger<AuthService> logger;
        private readonly IUsersService usersService;
        private readonly IPasswordHasher<User> passwordHasher;

        public AuthService(ILogger<AuthService> logger, IConfiguration configuration, IUsersService usersService, IPasswordHasher<User> passwordHasher)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
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
            var configurationKey = configuration["Jwt:Key"];

            var key = Encoding.ASCII.GetBytes(configurationKey!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userData.EmailId.ToString()),
                    new Claim(ClaimTypes.Name, userData.UserName),
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
                Issuer = configuration["Jwt:ValidIssuer"],
                Audience = configuration["Jwt:ValidAudience"]
            };

            return tokenHandler.CreateToken(tokenDescriptor);
        }

        public User RegisterUser(UserRegistrationRequest request)
        {
            string salt = GenerateSalt();
            string saltedPassword = request.Password + salt;
            string hashedPassword = passwordHasher.HashPassword(null!, saltedPassword);
            var user = new User { EmailId = request.EmailId, PasswordHash = hashedPassword, PasswordSalt = salt, UserName = request.UserName };
            return usersService.AddUser(user);
        }

        public bool ValidatePassword(UserLoginRequest loginRequest)
        {
            User user = usersService.GetUserByEmailId(loginRequest.EmailId);

            var saltedPassword = loginRequest.Password + user.PasswordSalt;
            PasswordVerificationResult result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, saltedPassword);

            if (result != PasswordVerificationResult.Success)
            {                
                return false;
            }

            return true;
        }
    }
}
