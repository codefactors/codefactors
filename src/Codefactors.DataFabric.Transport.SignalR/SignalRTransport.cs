// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Microsoft.AspNetCore.SignalR;

namespace Codefactors.DataFabric.Transport.SignalR;

/// <summary>
/// Data fabric transport using SignalR.
/// </summary>
/// <typeparam name="T">Type of SignalR hub.</typeparam>
public class SignalRTransport<T> : IDataFabricTransport
    where T : Hub
{
    private readonly IHubContext<T> _hubContext;
    private readonly ILogger<SignalRTransport<T>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SignalRTransport{T}"/> class.
    /// </summary>
    /// <param name="hubContext">SignalR hub context.</param>
    /// <param name="logger">Logger.</param>
    public SignalRTransport(IHubContext<T> hubContext, ILogger<SignalRTransport<T>> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    /// <summary>
    /// Sends an update to a subscription.
    /// </summary>
    /// <param name="subscriptionKey">Subscription key.</param>
    /// <param name="subscriptionPath">Path of subscription.</param>
    /// <param name="update">Update content.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task SendAsync(string subscriptionKey, string subscriptionPath, object update)
    {
        _logger.LogInformation("SignalR transport sending update notification for path '{path}' to user with subscription key '{key}'", subscriptionPath, subscriptionKey);

        await _hubContext.Clients.User(subscriptionKey).SendAsync("NotifyUpdate", new { Subscription = subscriptionPath, Data = update });
    }
}