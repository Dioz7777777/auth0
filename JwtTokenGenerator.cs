using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace SampleMvcApp;

public static class JwtTokenGenerator
{
    private const string BearerPrefix = "Bearer ";
    private static readonly SymmetricSecurityKey SecurityKey = new("nxALNfPIJjQSvPcv6qoMlrDnIB7Chs2p"u8.ToArray());
    private static readonly JwtSecurityTokenHandler TokenHandler = new();

    public static string GenerateToken(string username)
    {
        var credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, username)
        };
        var token = new JwtSecurityToken("http://localhost:3000/",
            "http://localhost:3000/",
            claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials);


        return TokenHandler.WriteToken(token);
    }

    public static bool DecodeToken(string securityToken, out Claim oClaim)
    {
        var accessTokenValue = securityToken.StartsWith(BearerPrefix)
            ? securityToken.Replace(BearerPrefix, string.Empty)
            : securityToken;

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = SecurityKey,
            ValidateIssuer = true,
            ValidIssuer = "http://localhost:3000/",
            ValidateAudience = true,
            ValidAudience = "http://localhost:3000/",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var principal = TokenHandler.ValidateToken(accessTokenValue, validationParameters, out var validatedToken);
            oClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            return true;
        }
        catch (Exception e)
        {
            oClaim = null;
            return false;
        }
    }
}