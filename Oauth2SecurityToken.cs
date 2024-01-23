using System;
using Microsoft.IdentityModel.Tokens;

namespace SampleMvcApp;

public sealed class Oauth2SecurityToken(string token) : SecurityToken
{
    public override string Id { get; } = token;
    public override string Issuer => string.Empty;
    public override SecurityKey SecurityKey { get; } = new JsonWebKey();
    public override SecurityKey SigningKey { get; set; } = new JsonWebKey();
    public override DateTime ValidFrom { get; } = DateTime.UtcNow.AddDays(-1);
    public override DateTime ValidTo => DateTime.UtcNow.AddDays(1);
}