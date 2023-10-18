using System;
using Microsoft.Extensions.DependencyInjection;
using SampleMvcApp.Authentication;

namespace SampleMvcApp.Extensions;

public static class ServiceCollectionExtensions
{
    public static Auth0WebAppAuthenticationBuilder AddAuth0WebAppAuthentication(
        this IServiceCollection services, Action<Auth0WebAppOptions> configureOptions) =>
        services.AddAuth0WebAppAuthentication(Auth0Constants.AuthenticationScheme, configureOptions);

    private static Auth0WebAppAuthenticationBuilder AddAuth0WebAppAuthentication(
        this IServiceCollection services, string authenticationScheme, Action<Auth0WebAppOptions> configureOptions) =>
        services.AddAuthentication(options =>
            {
                var auth0WebAppOptions = new Auth0WebAppOptions();
                configureOptions(auth0WebAppOptions);
                options.DefaultAuthenticateScheme = auth0WebAppOptions.CookieAuthenticationScheme;
                options.DefaultSignInScheme = auth0WebAppOptions.CookieAuthenticationScheme;
                options.DefaultChallengeScheme = auth0WebAppOptions.CookieAuthenticationScheme;
            })
            .AddAuth0WebAppAuthentication(authenticationScheme, configureOptions);
}