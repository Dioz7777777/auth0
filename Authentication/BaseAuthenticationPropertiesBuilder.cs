using Microsoft.AspNetCore.Authentication;

namespace SampleMvcApp.Authentication;

public abstract class BaseAuthenticationPropertiesBuilder
{
    protected readonly AuthenticationProperties AuthenticationProperties;

    protected BaseAuthenticationPropertiesBuilder(AuthenticationProperties? properties = null)
    {
        AuthenticationProperties = properties ?? new AuthenticationProperties();
        var authenticationProperties = AuthenticationProperties;
        if (authenticationProperties.RedirectUri != null)
            return;
        authenticationProperties.RedirectUri = "/";
    }
}