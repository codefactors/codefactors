// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Codefactors.Common.Model;
using Microsoft.Extensions.Primitives;

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
    /// <param name="queryParameters">Array containing query parameters as key/value pairs. May be empty.</param>
    /// <returns>Current data for the subscription.</returns>
    Task<object> AddSubscriptionAsync(
        IRequestContext requestContext,
        string path,
        IEnumerable<KeyValuePair<string, StringValues>> queryParameters);

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