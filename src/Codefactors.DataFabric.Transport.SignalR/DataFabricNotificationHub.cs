// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Codefactors.DataFabric.Subscriptions;
using Microsoft.AspNetCore.SignalR;

namespace Codefactors.DataFabric.Transport.SignalR;

/// <summary>
/// SignalR notification hub for sending notifications to clients over the SignalR data fabric transport.
/// </summary>
public class DataFabricNotificationHub : Hub
{
    private readonly ISubscriptionManager _subscriptionManager;
    private readonly ILogger<DataFabricNotificationHub> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataFabricNotificationHub"/> class.
    /// </summary>
    /// <param name="subscriptionManager">Subscription manager.</param>
    /// <param name="logger">Logger.</param>
    public DataFabricNotificationHub(ISubscriptionManager subscriptionManager, ILogger<DataFabricNotificationHub> logger)
    {
        _subscriptionManager = subscriptionManager;
        _logger = logger;

        _logger.LogInformation("DataFabricNotificationHub created");
    }

    /// <summary>
    /// Called when a new connection is established with the hub.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous connect.</returns>
    public async override Task OnConnectedAsync()
    {
        _logger.LogInformation("Connected");

        await base.OnConnectedAsync();

        await Clients.Caller.SendAsync("Message", "Connected successfully!");
    }

    /// <summary>
    /// Blah method.
    /// </summary>
    /// <param name="text">Text.</param>
    /// <returns>Task.</returns>
    public async Task Blah(string text)
    {
        _logger.LogInformation("Blah called with text " + text);
        await Clients.Caller.SendAsync("Message", "Blah called");
    }
}
