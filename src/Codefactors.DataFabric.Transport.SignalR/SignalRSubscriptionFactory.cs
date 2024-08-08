// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Codefactors.DataFabric.Subscriptions;

namespace Codefactors.DataFabric.Transport.SignalR;

/// <summary>
/// Factory for creating SignalR subscriptions.
/// </summary>
/// <param name="subscriptionKeyGenerator">Subscription key generator.</param>
/// <param name="transport">Data fabric transport.</param>
public class SignalRSubscriptionFactory(ISubscriptionKeyGenerator subscriptionKeyGenerator, IDataFabricTransport transport) : ISubscriptionFactory
{
    private readonly ISubscriptionKeyGenerator _subscriptionKeyGenerator = subscriptionKeyGenerator;
    private readonly IDataFabricTransport _transport = transport;

    /// <summary>
    /// Creates a new subscription.
    /// </summary>
    /// <param name="requestContext">Request context for the subscriber.</param>
    /// <param name="subscriptionPath">Subscription path.</param>
    /// <returns>New subscription.</returns>
    public ISubscription Create(IRequestContext requestContext, string subscriptionPath) =>
        new SignalRSubscription(
            subscriptionPath,
            _subscriptionKeyGenerator.GenerateKey(requestContext),
            _transport);
}