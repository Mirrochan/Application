using Infrastructure.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Event_Management_System.Extensions
{
    public static class ApiExtensions
    {

        public static void AddJwtAuthentication(this IServiceCollection services, IOptions<JwtOptions> jwtOptions)
        {
            var key = Encoding.UTF8.GetBytes(jwtOptions.Value.SecretKey);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Value.Issuer,
                    ValidAudience = jwtOptions.Value.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    
                    NameClaimType = "userId",
                    RoleClaimType = "role"
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        Console.WriteLine($"=== OnMessageReceived ===");
                        Console.WriteLine($"Path: {context.Request.Path}");
                        Console.WriteLine($"Cookies present: {context.Request.Cookies.ContainsKey("tasty-cookies")}");

                        
                        if (context.Request.Cookies.ContainsKey("tasty-cookies"))
                        {
                            context.Token = context.Request.Cookies["tasty-cookies"];
                            Console.WriteLine($"Token from cookies: {!string.IsNullOrEmpty(context.Token)}");
                            if (!string.IsNullOrEmpty(context.Token))
                            {
                                Console.WriteLine($"Token first 50 chars: {context.Token.Substring(0, Math.Min(50, context.Token.Length))}...");
                            }
                        }
              
                        else if (context.Request.Headers.ContainsKey("Authorization"))
                        {
                            var authHeader = context.Request.Headers["Authorization"].ToString();
                            Console.WriteLine($"Authorization header: {authHeader}");
                            if (authHeader.StartsWith("Bearer "))
                            {
                                context.Token = authHeader.Substring(7);
                                Console.WriteLine($"Token from header: {!string.IsNullOrEmpty(context.Token)}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No token found in cookies or headers");
                        }
                        Console.WriteLine($"=========================");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine($"=== OnTokenValidated ===");
                        Console.WriteLine($"Token validated successfully: {context.Principal?.Identity?.IsAuthenticated}");
                        if (context.Principal?.Identity?.IsAuthenticated == true)
                        {
                            Console.WriteLine($"Claims count: {context.Principal.Claims.Count()}");
                            foreach (var claim in context.Principal.Claims)
                            {
                                Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
                            }
                        }
                        Console.WriteLine($"=========================");
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"=== OnAuthenticationFailed ===");
                        Console.WriteLine($"Exception: {context.Exception.Message}");
                        Console.WriteLine($"Inner Exception: {context.Exception.InnerException?.Message}");
                        Console.WriteLine($"=========================");
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        Console.WriteLine($"=== OnChallenge ===");
                        Console.WriteLine($"Error: {context.Error}");
                        Console.WriteLine($"Description: {context.ErrorDescription}");
                        Console.WriteLine($"=========================");
                        return Task.CompletedTask;
                    }
                };
            });
        }
    }
}
