using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace SampleMvcApp.Authentication;

internal class JwtTokenFactory
{
    private readonly SecurityKey _securityKey;
    private readonly string _algorithm;

    public JwtTokenFactory(SecurityKey securityKey, string algorithm)
    {
        _securityKey = securityKey;
        _algorithm = algorithm;
    }

    public string GenerateToken(string issuer, string audience, string sub)
    {
        var signingCredentials = new SigningCredentials(_securityKey, _algorithm);
        var securityTokenHandler = new JwtSecurityTokenHandler();
        return securityTokenHandler.WriteToken(securityTokenHandler.CreateToken(CreateSecurityTokenDescriptor(issuer, audience, sub, signingCredentials)));
    }

    private static SecurityTokenDescriptor CreateSecurityTokenDescriptor(
        string issuer,
        string audience,
        string sub,
        SigningCredentials signingCredentials)
    {
        return new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new(nameof (sub), sub),
                new("jti", Guid.NewGuid().ToString())
            }),
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddSeconds(180.0),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = signingCredentials
        };
    }
}