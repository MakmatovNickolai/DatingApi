using Auth.Config;
using Auth.Services;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Auth;

public static class Extensions
{
    public static IServiceCollection AddAuth(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection("JwtBearerToken");
        serviceCollection.Configure<JwtOptions>(jwtSection);
        var jwtSettings = jwtSection.Get<JwtOptions>();

        serviceCollection.Configure<UserValidation>(configuration.GetSection("UserValidation"));
        serviceCollection.Configure<LinkOptions>(configuration.GetSection("Links"));

        var secretKey = Encoding.ASCII.GetBytes(jwtSettings!.SecretKey);

        serviceCollection.AddAuthentication()
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, x =>
            {
                x.RequireHttpsMetadata = false;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        serviceCollection
            .AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(
                        JwtBearerDefaults.AuthenticationScheme)
                    .Build();
            })
            .AddScoped<IRegisterService, RegisterService>()
            .AddScoped<IUserValidatorService, UserValidatorService>()
            .AddScoped<IVerificationCodeService, VerificationCodeService>()
            .AddScoped<IJwtTokenService, JwtTokenService>()
            .AddScoped<IAuthenticationService, AuthenticationService>()
            .AddScoped<IRestorePasswordService, RestorePasswordService>();

        return serviceCollection;
    }
}