using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace SampleMvcApp.Authentication;

public class Auth0WebAppOptions
{
    public string CookieAuthenticationScheme { get; set; } = "Cookies";

    public string Domain { get; set; }

    public string ClientId { get; set; }

    public string? ClientSecret { get; set; }

    public SecurityKey? ClientAssertionSecurityKey { get; set; }

    public string? ClientAssertionSecurityKeyAlgorithm { get; set; }

    public string Scope { get; set; } = "openid profile";

    public string? CallbackPath { get; set; }

    public bool SkipCookieMiddleware { get; set; }

    public string? Organization { get; set; }

    public IDictionary<string, string>? LoginParameters { get; set; } = new Dictionary<string, string>();

    public OpenIdConnectEvents? OpenIdConnectEvents { get; set; }

    public string? ResponseType { get; set; }

    public HttpClient? Backchannel { get; set; }

    public TimeSpan? MaxAge { get; set; }
}