using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Wbskt.Core.Web
{
    public static class Auth
    {
        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            var key = config["Jwt:Key"]!;
            var serverKey = config["Jwt:ServerKey"]!;
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                };
            })
            .AddJwtBearer("Server", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(serverKey)),
                };
            });
        }

        public static int GetUserId(this IPrincipal principal)
        {
            if (principal.Identity is not ClaimsIdentity claimsPrincipal)
                throw new AuthenticationException("Unable to get userId");

            var claim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type.Equals("userId", StringComparison.InvariantCulture));
            if (claim == null)
                throw new AuthenticationException("Unable to get userId");

            return int.Parse(claim.Value);
        }
    }
}
