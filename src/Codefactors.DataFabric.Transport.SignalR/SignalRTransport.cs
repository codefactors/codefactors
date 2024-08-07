// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Microsoft.AspNetCore.SignalR;

namespace Codefactors.DataFabric.Transport.SignalR;

public class SignalRTransport : IDataFabricTransport
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public SignalRTransport(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendAsync(string subscriptionKey, string subscriptionPath, object update)
    {
        await _hubContext.Clients.User(subscriptionKey).SendAsync("Notify", new { Subscription = subscriptionPath, Data = update });
    }
}
