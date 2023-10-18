using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using SampleMvcApp.Exceptions;

namespace SampleMvcApp.Authentication;

internal static class IdTokenValidator
{
    public static void Validate(Auth0WebAppOptions auth0Options, JwtSecurityToken token, IDictionary<string, string?>? properties = null)
    {
        var property = properties == null || !properties.ContainsKey(Auth0AuthenticationParameters.Organization)
            ? null
            : properties[Auth0AuthenticationParameters.Organization];

        if (!string.IsNullOrWhiteSpace(property))
        {
            var organizationClaim = property.StartsWith("org_") ? "org_id" : "org_name";
            var str1 = token.Claims.SingleOrDefault<Claim>(claim => claim.Type == organizationClaim)?.Value;
            var str2 = organizationClaim == "org_name" ? str1?.ToLower() : str1;
            var str3 = organizationClaim == "org_name" ? property.ToLower() : property;
            if (string.IsNullOrWhiteSpace(str2))
                throw new IdTokenValidationException("Organization claim (" + organizationClaim + ") must be a string present in the ID token.");
            if (str2 != str3)
            {
                var interpolatedStringHandler = new DefaultInterpolatedStringHandler(70, 3);
                interpolatedStringHandler.AppendLiteral("Organization claim (");
                interpolatedStringHandler.AppendFormatted(organizationClaim);
                interpolatedStringHandler.AppendLiteral(") mismatch in the ID token; expected \"");
                interpolatedStringHandler.AppendFormatted(str3);
                interpolatedStringHandler.AppendLiteral("\", found \"");
                interpolatedStringHandler.AppendFormatted(str2);
                interpolatedStringHandler.AppendLiteral("\".");
                throw new IdTokenValidationException(interpolatedStringHandler.ToStringAndClear());
            }
        }

        if (token.Claims.SingleOrDefault<Claim>(claim => claim.Type == "sub")?.Value == null)
            throw new IdTokenValidationException("Subject (sub) claim must be a string present in the ID token.");
        if (token.Claims.SingleOrDefault<Claim>(claim => claim.Type == "iat")?.Value == null)
            throw new IdTokenValidationException("Issued At (iat) claim must be an integer present in the ID token.");
        if (token.Audiences.Count<string>() > 1)
        {
            if (string.IsNullOrWhiteSpace(token.Payload.Azp))
                throw new IdTokenValidationException(
                    "Authorized Party (azp) claim must be a string present in the ID token when Audiences (aud) claim has multiple values.");
            if (token.Payload.Azp != auth0Options.ClientId)
            {
                var interpolatedStringHandler = new DefaultInterpolatedStringHandler(77, 2);
                interpolatedStringHandler.AppendLiteral("Authorized Party (azp) claim mismatch in the ID token; expected \"");
                interpolatedStringHandler.AppendFormatted(auth0Options.ClientId);
                interpolatedStringHandler.AppendLiteral("\", found \"");
                interpolatedStringHandler.AppendFormatted(token.Payload.Azp);
                interpolatedStringHandler.AppendLiteral("\".");
                throw new IdTokenValidationException(interpolatedStringHandler.ToStringAndClear());
            }
        }

        if (!auth0Options.MaxAge.HasValue) return;

        var str = token.Claims.SingleOrDefault<Claim>(claim => claim.Type == "auth_time")?.Value;
        long? nullable1 = (!string.IsNullOrWhiteSpace(str) ? (long)Convert.ToDouble(str, CultureInfo.InvariantCulture) : new long?()) ??
                          throw new IdTokenValidationException(
                              "Authentication Time (auth_time) claim must be an integer present in the ID token when MaxAge specified.");
        var nullable2 = new double?(nullable1.GetValueOrDefault());
        var totalSeconds = auth0Options.MaxAge.Value.TotalSeconds;
        var num = (long)(nullable2 + totalSeconds)!.Value;
        var intDate = EpochTime.GetIntDate(DateTime.Now);
        if (intDate > num)
        {
            var interpolatedStringHandler = new DefaultInterpolatedStringHandler(173, 2);
            interpolatedStringHandler.AppendLiteral(
                "Authentication Time (auth_time) claim in the ID token indicates that too much time has passed since the last end-user authentication. Current time (");
            interpolatedStringHandler.AppendFormatted(intDate);
            interpolatedStringHandler.AppendLiteral(") is after last auth at ");
            interpolatedStringHandler.AppendFormatted(num);
            interpolatedStringHandler.AppendLiteral(".");
            throw new IdTokenValidationException(interpolatedStringHandler.ToStringAndClear());
        }
    }
}