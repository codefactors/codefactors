// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Codefactors.Authentication.Jwt.Clerk;

/// <summary>
/// Options for <em>AddJwtBearer</em> extension method for Clerk JWT authentication.  Unlike AzureAD, Clerk
/// doesn't use the audience claim but rather uses an azp claim to validate the caller.
/// </summary>
public static class ClerkJwtBearerOptions
{
    private const string ClerkAuthorityKey = "Clerk:Authority";
    private const string ClerkAuthorizedPartiesKey = "Clerk:AuthorizedParties";

    /// <summary>
    /// Updates the supplied <see cref="JwtBearerOptions"/> instance with the necessary settings to support
    /// Clerk JWTs, using the settings provided in the supplied configuration.
    /// </summary>
    /// <param name="options"><see cref="JwtBearerOptions"/> to update.</param>
    /// <param name="configuration">Configuration to use.  Should contain Clerk:Authority and
    /// Clerk:AuthorizedParties entries.</param>
    /// <returns>Updated <see cref="JwtBearerOptions"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if either of the configuration parameters are missing.</exception>
    /// <exception cref="ArgumentNullException">Thrown if either parameter is null.</exception>
    public static JwtBearerOptions FromConfiguration(JwtBearerOptions options, ConfigurationManager configuration)
    {
        ArgumentNullException.ThrowIfNull(nameof(options));
        ArgumentNullException.ThrowIfNull(nameof(configuration));

        // Authority is the URL of our clerk instance
        var authority = configuration[ClerkAuthorityKey];

        if (string.IsNullOrEmpty(authority))
            throw new ArgumentException($"{ClerkAuthorityKey} configuration is missing or invalid", nameof(configuration));

        var authorizedParties = configuration.GetSection(ClerkAuthorizedPartiesKey).Get<string[]>();

        if (authorizedParties == null || authorizedParties.Length == 0)
            throw new ArgumentException($"{ClerkAuthorizedPartiesKey} configuration is missing or invalid", nameof(configuration));

        options.Authority = authority;

        options.TokenValidationParameters = new TokenValidationParameters()
        {
            // Disable audience validation as we aren't using it
            ValidateAudience = false,
            NameClaimType = ClaimTypes.NameIdentifier
        };

        options.Events = new JwtBearerEvents()
        {
            // Additional validation for AZP claim
            OnTokenValidated = context =>
            {
                var azpClaim = context.Principal?.FindFirstValue("azp");

                // AuthorizedParty is the base URL of our frontend.
                if (string.IsNullOrEmpty(azpClaim))
                    context.Fail("AZP Claim is missing");

                if (!authorizedParties.Contains(azpClaim))
                    context.Fail("AZP Claim is invalid");

                return Task.CompletedTask;
            }
        };

        return options;
    }
}
