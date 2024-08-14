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
public class SignalRTransport : IDataFabricTransport
{
    private readonly IHubContext<DataFabricNotificationHub> _hubContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="SignalRTransport"/> class.
    /// </summary>
    /// <param name="hubContext">SignalR hub context.</param>
    public SignalRTransport(IHubContext<DataFabricNotificationHub> hubContext)
    {
        _hubContext = hubContext;
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
        await _hubContext.Clients.User(subscriptionKey).SendAsync("Notify", new { Subscription = subscriptionPath, Data = update });
    }
}
