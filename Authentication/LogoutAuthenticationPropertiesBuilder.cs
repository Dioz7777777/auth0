using Microsoft.AspNetCore.Authentication;

namespace SampleMvcApp.Authentication;

public class LogoutAuthenticationPropertiesBuilder : BaseAuthenticationPropertiesBuilder
{
    public LogoutAuthenticationPropertiesBuilder(AuthenticationProperties? properties = null)
        : base(properties)
    {
    }

    public LogoutAuthenticationPropertiesBuilder WithRedirectUri(string redirectUri)
    {
        AuthenticationProperties.RedirectUri = redirectUri;
        return this;
    }

    public AuthenticationProperties Build() => AuthenticationProperties;
}