// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.DataFabric.Subscriptions;

public class SubscriptionCollection : ConcurrentCollection<ISubscription>
{
    public async Task NotifyAllAsync(string subscriptionPath, object update) =>
        await ForEachAsync(subscription => NotifyAsync(subscription, subscriptionPath, update));

    private async Task NotifyAsync(ISubscription subscription, string subscriptionPath, object update) =>
        await subscription.NotifyAsync(subscriptionPath, update);
}