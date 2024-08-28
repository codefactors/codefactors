// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.DataFabric.Subscriptions;

/// <summary>
/// Collection container for subscriptions.
/// </summary>
public class SubscriptionCollection : ConcurrentCollection<ISubscription>
{
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionCollection"/> class.
    /// </summary>
    /// <param name="logger">Logger.</param>
    public SubscriptionCollection(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Notifies all subscriptions of an update.
    /// </summary>
    /// <param name="subscriptionPath">Subscription path.</param>
    /// <param name="update">Update content.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task NotifyAllAsync(string subscriptionPath, object update) =>
        await ForEachAsync(subscription => NotifyAsync(subscription, subscriptionPath, update));

    private async Task NotifyAsync(ISubscription subscription, string subscriptionPath, object update)
    {
        _logger.LogInformation("NotifyAsync: path '{path}', subscription '{subscription}'", subscriptionPath, subscription);

        await subscription.NotifyAsync(subscriptionPath, update);
    }
}