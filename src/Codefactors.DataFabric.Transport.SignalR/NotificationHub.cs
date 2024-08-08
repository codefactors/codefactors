// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Codefactors.DataFabric.Subscriptions;
using Microsoft.AspNetCore.SignalR;

namespace Codefactors.DataFabric.Transport.SignalR;

public class NotificationHub : Hub
{
    private readonly ISubscriptionManager _subscriptionManager;
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ISubscriptionManager subscriptionManager, ILogger<NotificationHub> logger)
    {
        _subscriptionManager = subscriptionManager;
        _logger = logger;

        _logger.LogInformation("NotificationHub created");
    }

    public async override Task OnConnectedAsync()
    {
        _logger.LogInformation("Connected");
        await base.OnConnectedAsync();
        await Clients.Caller.SendAsync("Message", "Connected successfully!");
    }

    public async Task Blah(string text)
    {
        _logger.LogInformation("Blah called with text " + text);
        await Clients.Caller.SendAsync("Message", "Blah called");
    }
}
