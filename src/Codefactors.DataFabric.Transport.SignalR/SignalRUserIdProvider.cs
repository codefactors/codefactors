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
/// <param name="logger">Logger.</param>
public class SignalRUserIdProvider(
    ISubscriptionKeyGenerator subscriptionKeyGenerator,
    ILogger<IUserIdProvider> logger) : IUserIdProvider
{
    private readonly ISubscriptionKeyGenerator _subscriptionKeyGenerator = subscriptionKeyGenerator;
    private readonly ILogger _logger = logger;

    /// <summary>
    /// Gets the user ID for the specified connection.
    /// </summary>
    /// <param name="connection">The connection to get the user ID for.</param>
    /// <returns>The user ID for the specified connection.</returns>
    public string GetUserId(HubConnectionContext connection)
    {
        var userId = _subscriptionKeyGenerator is IClaimsBasedSubscriptionKeyGenerator generator &&
            connection.User?.Claims is IEnumerable<Claim> claims ?
                generator.GenerateKey(claims) :
                string.Empty;

        _logger.LogInformation("SignalR UserId = {userId}", userId);

        return userId;
    }
}