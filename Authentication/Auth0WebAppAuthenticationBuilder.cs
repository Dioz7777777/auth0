using Microsoft.Extensions.DependencyInjection;

namespace SampleMvcApp.Authentication;

public class Auth0WebAppAuthenticationBuilder
{
    private readonly IServiceCollection _services;
    private readonly Auth0WebAppOptions _options;
    private readonly string _authenticationScheme;

    public Auth0WebAppAuthenticationBuilder(
        IServiceCollection services,
        string authenticationScheme,
        Auth0WebAppOptions options)
    {
        _services = services;
        _options = options;
        _authenticationScheme = authenticationScheme;
    }
}