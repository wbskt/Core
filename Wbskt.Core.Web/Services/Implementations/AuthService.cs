﻿using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Wbskt.Common;
using Wbskt.Common.Contracts;

namespace Wbskt.Core.Web.Services.Implementations;

public class AuthService(ILogger<AuthService> logger, IConfiguration configuration, IUsersService usersService, IPasswordHasher<User> passwordHasher) : IAuthService
{
    private readonly IConfiguration configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    private readonly ILogger<AuthService> logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IUsersService usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
    private readonly IPasswordHasher<User> passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));

    public string GenerateToken(User userData)
    {
        var tokenHandler = new JsonWebTokenHandler();
        var configurationKey = configuration[Constants.JwtKeyNames.UserTokenKey];

        var key = Encoding.UTF8.GetBytes(configurationKey!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(Constants.Claims.EmailId, userData.EmailId),
                new Claim(Constants.Claims.Name, userData.UserName),
                new Claim(Constants.Claims.UserData, userData.UserId.ToString())
            }),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
        };

        return tokenHandler.CreateToken(tokenDescriptor);
    }

    public string CreateCoreServerToken()
    {
        var tokenHandler = new JsonWebTokenHandler();
        var configurationKey = configuration[Constants.JwtKeyNames.CoreServerTokenKey];

        var key = Encoding.UTF8.GetBytes(configurationKey!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(Constants.Claims.CoreServer, Guid.NewGuid().ToString())
            }),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
            Issuer = "wbskt.core",
            Expires = DateTime.Now.AddMinutes(Constants.ExpiryTimes.ServerTokenExpiry)
        };

        logger.LogDebug("core server token created");
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
        PasswordVerificationResult result = passwordHasher.VerifyHashedPassword(null!, user.PasswordHash, saltedPassword);

        if (result != PasswordVerificationResult.Success)
        {
            return false;
        }

        return true;
    }

    private static string GenerateSalt()
    {
        var buffer = new byte[16];
        RandomNumberGenerator.Fill(buffer);

        return Convert.ToBase64String(buffer);
    }
}
