using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SampleMvcApp.Authentication;

namespace SampleMvcApp.Extensions;

public static class AuthenticationBuilderExtensions
{
    private static readonly IList<string> CodeResponseTypes = new List<string>
    {
        "code",
        "code id_token"
    };

    public static Auth0WebAppAuthenticationBuilder AddAuth0WebAppAuthentication(
        this AuthenticationBuilder builder,
        Action<Auth0WebAppOptions> configureOptions) =>
        builder.AddAuth0WebAppAuthentication(Auth0Constants.AuthenticationScheme, configureOptions);

    public static Auth0WebAppAuthenticationBuilder AddAuth0WebAppAuthentication(
        this AuthenticationBuilder builder,
        string authenticationScheme,
        Action<Auth0WebAppOptions> configureOptions)
    {
        var auth0Options = new Auth0WebAppOptions();
        configureOptions(auth0Options);
        ValidateOptions(auth0Options);
        builder.AddOpenIdConnect(authenticationScheme,
            options => ConfigureOpenIdConnect(options, auth0Options));
        if (!auth0Options.SkipCookieMiddleware)
            builder.AddCookie(auth0Options.CookieAuthenticationScheme);
        builder.Services.Configure(authenticationScheme, configureOptions);
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<OpenIdConnectOptions>, Auth0OpenIdConnectPostConfigureOptions>());
        return new Auth0WebAppAuthenticationBuilder(builder.Services, authenticationScheme, auth0Options);
    }

    private static void ConfigureOpenIdConnect(
        OpenIdConnectOptions oidcOptions,
        Auth0WebAppOptions auth0Options)
    {
        oidcOptions.Authority = "https://" + auth0Options.Domain;
        oidcOptions.ClientId = auth0Options.ClientId;
        oidcOptions.ClientSecret = auth0Options.ClientSecret;
        oidcOptions.Scope.Clear();
        oidcOptions.Scope.AddRange(auth0Options.Scope.Split(" "));
        oidcOptions.CallbackPath = new PathString(auth0Options.CallbackPath ?? Auth0Constants.DefaultCallbackPath);
        oidcOptions.SaveTokens = true;
        oidcOptions.ResponseType = auth0Options.ResponseType ?? oidcOptions.ResponseType;
        oidcOptions.Backchannel = auth0Options.Backchannel!;
        oidcOptions.MaxAge = auth0Options.MaxAge;
        if (!oidcOptions.Scope.Contains("openid"))
            oidcOptions.Scope.Add("openid");
        oidcOptions.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = "name",
            ValidateAudience = true,
            ValidAudience = auth0Options.ClientId,
            ValidateIssuer = true,
            ValidIssuer = "https://" + auth0Options.Domain + "/",
            ValidateLifetime = true,
            RequireExpirationTime = true
        };
        oidcOptions.Events = OpenIdConnectEventsFactory.Create(auth0Options);
    }

    private static void ValidateOptions(Auth0WebAppOptions auth0Options)
    {
        if (CodeResponseTypes.Contains(auth0Options.ResponseType!) && string.IsNullOrWhiteSpace(auth0Options.ClientSecret))
            throw new ArgumentNullException("ClientSecret", "Client Secret can not be null when using `code` or `code id_token` as the response_type.");
    }
}