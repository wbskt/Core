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
            var key = config["Jwt:key"]!;
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["Jwt:ValidIssuer"],
                    ValidAudience = config["Jwt:ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key))
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
