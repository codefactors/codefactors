// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Codefactors.DataFabric.Subscriptions;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Codefactors.DataFabric.Transport.SignalR;

/// <summary>
/// Helper that provides the user id for SignalR connections.
/// </summary>
/// <param name="subscriptionKeyGenerator">Subscription key generator.</param>
public class SignalRUserIdProvider(ISubscriptionKeyGenerator subscriptionKeyGenerator) : IUserIdProvider
{
    private readonly ISubscriptionKeyGenerator _subscriptionKeyGenerator = subscriptionKeyGenerator;

    /// <summary>
    /// Gets the user ID for the specified connection.
    /// </summary>
    /// <param name="connection">The connection to get the user ID for.</param>
    /// <returns>The user ID for the specified connection.</returns>
    public string GetUserId(HubConnectionContext connection) =>
        _subscriptionKeyGenerator is IClaimsBasedSubscriptionKeyGenerator generator &&
        connection.User?.Claims is IEnumerable<Claim> claims ?
            generator.GenerateKey(claims) :
            string.Empty;
}

/// <summary>
/// Interface for entities that generate subscription keys.
/// </summary>
internal interface IClaimsBasedSubscriptionKeyGenerator : ISubscriptionKeyGenerator
{
    /// <summary>
    /// Generates the subscription key for the specified request context.
    /// </summary>
    /// <param name="claims">Request context.</param>
    /// <returns>Subscription key.</returns>
    string GenerateKey(IEnumerable<Claim> claims);
}
