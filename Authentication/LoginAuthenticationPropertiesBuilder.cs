using Microsoft.AspNetCore.Authentication;

namespace SampleMvcApp.Authentication;

public class LoginAuthenticationPropertiesBuilder : BaseAuthenticationPropertiesBuilder
{
    public LoginAuthenticationPropertiesBuilder(AuthenticationProperties? properties = null)
        : base(properties)
    {
    }

    public LoginAuthenticationPropertiesBuilder WithRedirectUri(string redirectUri)
    {
        AuthenticationProperties.RedirectUri = redirectUri;
        return this;
    }

    public AuthenticationProperties Build() => AuthenticationProperties;
}