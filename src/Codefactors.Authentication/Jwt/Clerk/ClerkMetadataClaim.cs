// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using System.Security.Claims;
using System.Text.Json;

namespace Codefactors.Authentication.Jwt.Clerk;

/// <summary>
/// Represents a Clerk metadata claim.
/// </summary>
public class ClerkMetadataClaim
{
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

    /// <summary>
    /// Gets the role value from the Clerk claim metadata.
    /// </summary>
    public string Role { get; init; } = default!;

    /// <summary>
    /// Deserialises the metadata claim from the supplied Clerk JWT token claims.
    /// </summary>
    /// <param name="claims">Clerk JWT token.</param>
    /// <returns><see cref="ClerkMetadataClaim"/> instance if the claim was present, null otherwise.</returns>
    public static ClerkMetadataClaim? DeserialiseMetadataClaim(IEnumerable<Claim>? claims)
    {
        if (claims != null)
        {
            var metadataJson = claims.FirstOrDefault(c => c.Type == "metadata")?.Value;

            if (!string.IsNullOrEmpty(metadataJson))
                return JsonSerializer.Deserialize<ClerkMetadataClaim>(metadataJson, _jsonOptions);
        }

        return null;
    }
}
