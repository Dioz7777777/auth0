using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;

namespace SampleMvcApp.Authentication;

internal class Auth0OpenIdConnectPostConfigureOptions : IPostConfigureOptions<OpenIdConnectOptions>
{
    public void PostConfigure(string? name, OpenIdConnectOptions options) =>
        options.Backchannel.DefaultRequestHeaders.Add("Auth0-Client", Utils.CreateAgentString());
}