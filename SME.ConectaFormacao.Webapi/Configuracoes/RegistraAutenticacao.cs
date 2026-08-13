using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using static System.Text.Encoding;

namespace SME.ConectaFormacao.Webapi.Configuracoes;

public static class RegistraAutenticacao
{
    public static void Registrar(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidAudience = configuration.GetValue<string>("JwtTokenSettings:Audience"),
                ValidateIssuer = true,
                ValidIssuer = configuration.GetValue<string>("JwtTokenSettings:Issuer"),
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                //IssuerSigningKey = new SymmetricSecurityKey(UTF8.GetBytes(configuration.GetValue<string>("JwtTokenSettings:IssuerSigningKey")))
                IssuerSigningKey = new SymmetricSecurityKey(UTF8.GetBytes("132CD0A7B1215897C5EF1CC6D7C7469C687D17FAZ85E675B6EBD9FBA26615B93805556652B2DDFD96CA2565C8D42EE83EF44CAC3B79AF64B343461B52ACC75FA"))
            };
        });

        services.AddAuthorization(auth =>
        {
            auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()

                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build());
        });
    }
}