using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SampleMvcApp.ViewModels;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using SampleMvcApp.DTO;

namespace SampleMvcApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly string _domain;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _callbackUrl;
        private readonly string _logoutUrl;

        public AccountController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _domain = configuration["Auth0:Domain"]!;
            _clientId = configuration["Auth0:ClientId"]!;
            _clientSecret = configuration["Auth0:ClientSecret"]!;
            _callbackUrl = configuration["Auth0:CallbackUrl"]!;
            _logoutUrl = configuration["Auth0:LogoutUrl"]!;
        }

        [HttpGet("login")]
        public IActionResult Login() => Redirect($"https://{_domain}/authorize?" +
                                                 $"client_id={_clientId}&" +
                                                 $"response_type=code&" +
                                                 $"redirect_uri={_callbackUrl}&" +
                                                 $"scope=openid profile email");

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            AuthState.AccessToken = null;
            return Redirect($"https://{_domain}/v2/logout?" +
                            $"client_id={_clientId}&" +
                            $"returnTo={_logoutUrl}");
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string code)
        {
            var client = _httpClientFactory.CreateClient();
            var tokenResponse = await client.PostAsync($"https://{_domain}/oauth/token", new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("client_secret", _clientSecret),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", _callbackUrl)
            }));

            var token = await tokenResponse.Content.ReadFromJsonAsync<GetTokenResponse>();
            if (string.IsNullOrEmpty(token?.AccessToken)) return Redirect("Home/Error");
            AuthState.AccessToken = token.AccessToken;
            return Redirect("Home/Index");
        }

        public async Task<IActionResult> Profile()
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {AuthState.AccessToken}");
            var userInfo = await client.GetFromJsonAsync<GetUserResponse>($"https://{_domain}/userinfo");

            return View(new UserProfileViewModel
            {
                Name = userInfo!.Name,
                EmailAddress = userInfo.EmailAddress,
                ProfileImage = userInfo.ProfileImage
            });
        }
    }
}