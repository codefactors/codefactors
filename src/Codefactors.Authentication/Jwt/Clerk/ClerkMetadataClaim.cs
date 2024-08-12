// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.Json;

namespace Codefactors.Authentication.Jwt.Clerk;

/// <summary>
/// Represents a Clerk metadata claim.
/// </summary>
public class ClerkMetadataClaim
{
    /// <summary>
    /// Type name of the metadata claim.
    /// </summary>
    public const string Type = "metadata";

    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

    /// <summary>
    /// Gets the role value from the Clerk claim metadata.
    /// </summary>
    public string Role { get; init; } = default!;

    /// <summary>
    /// Attempts to get the metadata claim from the supplied set of claims.
    /// </summary>
    /// <param name="principal">ClaimsPrincipal with Set of claims to search for the metadata claim.</param>
    /// <param name="claim">Metadata claim if present; otherwise null.</param>
    /// <returns>True if the claim was found, false otherwise.</returns>
    public static bool TryGetClaim(in ClaimsPrincipal principal, [NotNullWhen(true)] out Claim? claim) =>
        (claim = principal.Claims?.FirstOrDefault(c => c.Type == Type)) != null;

    /// <summary>
    /// Deserialises the metadata claim from the supplied Clerk JWT token claims.
    /// </summary>
    /// <param name="claim">Clerk metadata claim.</param>
    /// <param name="metadataClaim">Deserialised claim as <see cref="ClerkMetadataClaim"/> instance if deserialisation
    /// was possible; null otherwise.</param>
    /// <returns><see cref="ClerkMetadataClaim"/> instance if the claim could be deserialised, null otherwise.</returns>
    public static bool TryDeserialise(Claim claim, [NotNullWhen(true)] out ClerkMetadataClaim? metadataClaim) =>
        (metadataClaim = !string.IsNullOrEmpty(claim.Value) ?
            JsonSerializer.Deserialize<ClerkMetadataClaim>(claim.Value, _jsonOptions) :
            null) != null;
}