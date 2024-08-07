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
    /// <summary>
    /// Notifies all subscriptions of an update.
    /// </summary>
    /// <param name="subscriptionPath">Subscription path.</param>
    /// <param name="update">Update content.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task NotifyAllAsync(string subscriptionPath, object update) =>
        await ForEachAsync(subscription => NotifyAsync(subscription, subscriptionPath, update));

    private async Task NotifyAsync(ISubscription subscription, string subscriptionPath, object update) =>
        await subscription.NotifyAsync(subscriptionPath, update);
}