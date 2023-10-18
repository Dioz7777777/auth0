using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using SampleMvcApp.Exceptions;

namespace SampleMvcApp.Authentication;

internal sealed class OpenIdConnectEventsFactory
{
    internal static OpenIdConnectEvents Create(Auth0WebAppOptions auth0Options) => new()
    {
        OnRedirectToIdentityProvider = ProxyEvent(auth0Options.OpenIdConnectEvents?.OnRedirectToIdentityProvider,
            CreateOnRedirectToIdentityProvider(auth0Options)),
        OnRedirectToIdentityProviderForSignOut = ProxyEvent(auth0Options.OpenIdConnectEvents?.OnRedirectToIdentityProviderForSignOut,
            CreateOnRedirectToIdentityProviderForSignOut(auth0Options)),
        OnTokenValidated = ProxyEvent(auth0Options.OpenIdConnectEvents?.OnTokenValidated, CreateOnTokenValidated(auth0Options)),
        OnAccessDenied = ProxyEvent(auth0Options.OpenIdConnectEvents?.OnAccessDenied),
        OnAuthenticationFailed = ProxyEvent(auth0Options.OpenIdConnectEvents?.OnAuthenticationFailed),
        OnAuthorizationCodeReceived = ProxyEvent(auth0Options.OpenIdConnectEvents?.OnAuthorizationCodeReceived,
            CreateOnAuthorizationCodeReceived(auth0Options)),
        OnMessageReceived = ProxyEvent(auth0Options.OpenIdConnectEvents?.OnMessageReceived),
        OnRemoteFailure = ProxyEvent(auth0Options.OpenIdConnectEvents?.OnRemoteFailure),
        OnRemoteSignOut = ProxyEvent(auth0Options.OpenIdConnectEvents?.OnRemoteSignOut),
        OnSignedOutCallbackRedirect = ProxyEvent(auth0Options.OpenIdConnectEvents?.OnSignedOutCallbackRedirect),
        OnTicketReceived = ProxyEvent(auth0Options.OpenIdConnectEvents?.OnTicketReceived),
        OnTokenResponseReceived = ProxyEvent(auth0Options.OpenIdConnectEvents?.OnTokenResponseReceived),
        OnUserInformationReceived = ProxyEvent(auth0Options.OpenIdConnectEvents?.OnUserInformationReceived)
    };

    private static Func<T, Task> ProxyEvent<T>(Func<T, Task>? originalHandler, Func<T, Task>? newHandler = null) => async context =>
    {
        if (newHandler != null)
            await newHandler(context);
        if (originalHandler == null)
            return;
        await originalHandler(context);
    };

    private static Func<RedirectContext, Task> CreateOnRedirectToIdentityProvider(Auth0WebAppOptions auth0Options) => context =>
    {
        context.ProtocolMessage.SetParameter("auth0Client", Utils.CreateAgentString());
        foreach (var (key, value) in GetAuthorizeParameters(auth0Options, context.Properties.Items))
            context.ProtocolMessage.SetParameter(key, value);
        if (!string.IsNullOrWhiteSpace(auth0Options.Organization) && !context.Properties.Items.ContainsKey(Auth0AuthenticationParameters.Organization))
            context.Properties.Items[Auth0AuthenticationParameters.Organization] = auth0Options.Organization;
        return Task.CompletedTask;
    };

    private static Func<RedirectContext, Task> CreateOnRedirectToIdentityProviderForSignOut(Auth0WebAppOptions auth0Options) => context =>
    {
        var location = "https://" + auth0Options.Domain + "/v2/logout?client_id=" + auth0Options.ClientId;
        var stringToEscape1 = context.Properties.RedirectUri;
        var extraParameters = GetExtraParameters(context.Properties.Items);
        if (!string.IsNullOrEmpty(stringToEscape1))
        {
            if (stringToEscape1.StartsWith("/"))
            {
                var request = context.Request;
                stringToEscape1 = request.Scheme + "://" + request.Host + request.PathBase + stringToEscape1;
            }

            location = location + "&returnTo=" + Uri.EscapeDataString(stringToEscape1);
        }

        foreach (var (key, stringToEscape2) in extraParameters)
        {
            if (!string.IsNullOrEmpty(stringToEscape2))
                location = location + "&" + key + "=" + Uri.EscapeDataString(stringToEscape2);
            else
                location = location + "&" + key;
        }

        context.Response.Redirect(location);
        context.HandleResponse();
        return Task.CompletedTask;
    };

    private static Func<TokenValidatedContext, Task> CreateOnTokenValidated(Auth0WebAppOptions auth0Options) => context =>
    {
        try
        {
            IdTokenValidator.Validate(auth0Options, context.SecurityToken, context.Properties?.Items);
        }
        catch (IdTokenValidationException ex)
        {
            context.Fail(ex.Message);
        }

        return Task.CompletedTask;
    };

    private static Func<AuthorizationCodeReceivedContext, Task> CreateOnAuthorizationCodeReceived(Auth0WebAppOptions auth0Options) => context =>
    {
        if (auth0Options.ClientAssertionSecurityKey != null)
        {
            context.TokenEndpointRequest?.SetParameter("client_assertion",
                new JwtTokenFactory(auth0Options.ClientAssertionSecurityKey, auth0Options.ClientAssertionSecurityKeyAlgorithm ?? "RS256").GenerateToken(
                    auth0Options.ClientId, "https://" + auth0Options.Domain + "/", auth0Options.ClientId));
            context.TokenEndpointRequest?.SetParameter("client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer");
        }

        return Task.CompletedTask;
    };

    private static IDictionary<string, string?> GetAuthorizeParameters(Auth0WebAppOptions auth0Options, IDictionary<string, string?> authSessionItems)
    {
        var authorizeParameters = new Dictionary<string, string>();
        if (!string.IsNullOrEmpty(auth0Options.Organization)) authorizeParameters["organization"] = auth0Options.Organization;

        if (auth0Options.LoginParameters != null)
        {
            foreach (var (key, str) in auth0Options.LoginParameters) authorizeParameters[key] = str;
        }

        foreach (var (key, value) in GetExtraParameters(authSessionItems))
        {
            var str = value;
            if (key == "scope")
            {
                if (str == null)
                    str = "openid";
                else if (!str.Contains("openid", StringComparison.CurrentCultureIgnoreCase))
                    str += " openid";
            }

            authorizeParameters[key] = str!;
        }

        return authorizeParameters!;
    }

    private static IDictionary<string, string?> GetExtraParameters(IDictionary<string, string> authSessionItems)
    {
        var extraParameters = new Dictionary<string, string?>();
        foreach (var (key, str) in authSessionItems.Where(
                     item => item.Key.StartsWith(Auth0AuthenticationParameters.Prefix + ":")))
            extraParameters[key.Replace(Auth0AuthenticationParameters.Prefix + ":", "")] = str;
        return extraParameters;
    }
}