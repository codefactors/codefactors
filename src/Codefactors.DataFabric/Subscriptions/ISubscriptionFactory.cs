// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Codefactors.Common.Model;

namespace Codefactors.DataFabric.Subscriptions;

/// <summary>
/// Factory for creating subscriptions.
/// </summary>
public interface ISubscriptionFactory
{
    /// <summary>
    /// Creates a new subscription.
    /// </summary>
    /// <param name="requestContext">Request context for the subscriber.</param>
    /// <param name="subscriptionPath">Subscription path.</param>
    /// <returns>New subscription.</returns>
    ISubscription Create(IRequestContext requestContext, string subscriptionPath);
}
