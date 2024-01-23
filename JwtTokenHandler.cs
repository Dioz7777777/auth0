using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SampleMvcApp.Managers;

namespace SampleMvcApp;

public sealed class JwtTokenHandler : JwtSecurityTokenHandler
{
    // private IUserManager _userManager = Injector.ServiceProvider.GetRequiredService(typeof(IUserManager)) as IUserManager;

    private const string BearerPrefix = "Bearer ";

    public override Task<TokenValidationResult> ValidateTokenAsync(SecurityToken token, TokenValidationParameters validationParameters) =>
        ValidateTokenAsync(token.UnsafeToString(), validationParameters);

    public override int MaximumTokenSizeInBytes { get; set; } = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;

    public override Task<TokenValidationResult> ValidateTokenAsync(string token, TokenValidationParameters validationParameters)
    {
        var isValid = CheckToken(token);
        var result = new TokenValidationResult
        {
            SecurityToken = new Oauth2SecurityToken(token),
            IsValid = isValid,
            ClaimsIdentity = new ClaimsIdentity(token)
        };
        return Task.FromResult(result);
    }

    private bool CheckToken(string securityToken)
    {
        if (JwtTokenGenerator.DecodeToken(securityToken, out var claims))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}