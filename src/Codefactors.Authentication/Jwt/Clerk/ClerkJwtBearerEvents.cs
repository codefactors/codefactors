// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net.Sockets;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace Codefactors.Authentication.Jwt.Clerk;

/// <summary>
/// Event handler for Clerk bearer token validation.
/// </summary>
public class ClerkJwtBearerEvents : JwtBearerEvents
{
    /// <summary>
    /// Events processor for Clerk JWT bearer token processing.
    /// </summary>
    /// <param name="authorisedParties">Array of authorised parties (typically URIs of calling parties).</param>
    public ClerkJwtBearerEvents(string[] authorisedParties)
    {
        OnTokenValidated = context =>
        {
            var azpClaim = context.Principal?.FindFirstValue("azp");

            // AuthorizedParty is the base URL of our frontend.
            if (string.IsNullOrEmpty(azpClaim))
                context.Fail("AZP Claim is missing");

            if (!authorisedParties.Contains(azpClaim))
                context.Fail("AZP Claim is invalid");

            if (context.Result?.Failure == null)
                ProcessMetadata(context);

            return Task.CompletedTask;
        };
    }

    private static Task ProcessMetadata(TokenValidatedContext content)
    {
        var context = content.HttpContext;

        var metadata = ClerkMetadataClaim.DeserialiseMetadataClaim(context.User.Claims);

        if (metadata != null && metadata.Role != null)
            (content.Principal?.Identity as ClaimsIdentity)?.AddClaim(new Claim(ClaimTypes.Role, metadata.Role));

        return Task.CompletedTask;
    }
}
