// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.DataFabric.Subscriptions;

/// <summary>
/// Interface that represents a subscription.
/// </summary>
public interface ISubscription : IEquatable<ISubscription>
{
    /// <summary>
    /// Notifies the subscription of an update.
    /// </summary>
    /// <param name="subscriptionPath">Path for the subscription being updated.</param>
    /// <param name="update">Update content.</param>
    /// <returns><see cref="Task"/>.</returns>
    public Task NotifyAsync(string subscriptionPath, object update);
}
