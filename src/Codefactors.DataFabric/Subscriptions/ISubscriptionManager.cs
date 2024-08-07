// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.DataFabric.Subscriptions;

/// <summary>
/// Interface that represents a subscription manager.
/// </summary>
public interface ISubscriptionManager
{
    /// <summary>
    /// Adds a subscription.
    /// </summary>
    /// <param name="requestContext">Request context.</param>
    /// <param name="path">Subscription path.</param>
    /// <returns>Current data for the subscription.</returns>
    Task<object> AddSubscriptionAsync(IRequestContext requestContext, string path);

    /// <summary>
    /// Removes a subscription.
    /// </summary>
    /// <param name="requestContext">Request context.</param>
    /// <param name="path">Subscription path.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task RemoveSubscriptionAsync(IRequestContext requestContext, string path);

    /// <summary>
    /// Notifies subscribers of an update.
    /// </summary>
    /// <param name="path">Subscription path.</param>
    /// <param name="update">Update content.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task NotifySubscribersAsync(string path, object update);
}