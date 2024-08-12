// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;

namespace Codefactors.Authentication.Jwt.Clerk;

/// <summary>
/// Event handler for Clerk bearer token validation.
/// </summary>
public class ClerkJwtBearerEvents : JwtBearerEvents
{
    private const string AuthorisedPartiesClaim = "azp";

    /// <summary>
    /// Events processor for Clerk JWT bearer token processing.
    /// </summary>
    /// <param name="authorisedParties">Array of authorised parties (typically URIs of calling parties).</param>
    public ClerkJwtBearerEvents(string[] authorisedParties)
    {
        OnTokenValidated = context =>
        {
            var azpClaim = context.Principal?.FindFirstValue(AuthorisedPartiesClaim);

            // AuthorizedParty is the base URL of our frontend.
            if (string.IsNullOrEmpty(azpClaim))
                context.Fail("AZP (authorised parties) claim is missing");

            if (!authorisedParties.Contains(azpClaim))
                context.Fail("AZP (authorised parties) claim is invalid");

            if (context.Result?.Failure == null)
                ProcessMetadataClaimIfPresent(context);

            return Task.CompletedTask;
        };
    }

    private static Task ProcessMetadataClaimIfPresent(TokenValidatedContext content)
    {
        if (content.Principal is ClaimsPrincipal principal &&
            ClerkMetadataClaim.TryGetClaim(principal, out var claim) &&
            ClerkMetadataClaim.TryDeserialise(claim, out var metadata) &&
            principal.Identity is ClaimsIdentity currentClaims)
        {
            if (metadata.Role != null)
                currentClaims.AddClaim(new Claim(ClaimTypes.Role, metadata.Role));

            currentClaims.RemoveClaim(claim);
        }

        return Task.CompletedTask;
    }
}
