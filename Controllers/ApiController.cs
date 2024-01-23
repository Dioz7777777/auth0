using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SampleMvcApp.Managers;

namespace SampleMvcApp.Controllers;

[Route("api")]
public sealed class ApiController(IUserManager userManager) : Controller
{
    [Authorize]
    [HttpGet("time")]
    public async Task<IActionResult> GetTimeZone()
    {
        var authToken = HttpContext.Request.Headers.Authorization.ToString();
        if (!JwtTokenGenerator.DecodeToken(authToken, out var claim)) return Unauthorized();

        var username = claim?.Value;
        var user = await userManager.GetByName(username);
        if (user == null) return Unauthorized();

        return Ok(user.Timezone);
    }

    [Authorize]
    [HttpPut("user/profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileViewModel model)
    {
        var authToken = HttpContext.Request.Headers.Authorization.ToString();
        if (!JwtTokenGenerator.DecodeToken(authToken, out var claim)) return Unauthorized();

        var username = claim?.Value;
        var user = await userManager.GetByName(username);
        if (user == null) return Unauthorized();

        user.Timezone = model.Timezone;
        await userManager.Update(user);

        return Ok();
    }
}

public sealed record UpdateProfileViewModel
{
    public string Timezone { get; init; } = string.Empty;
}