// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.DataFabric.Transport.SignalR;

public class SignalRSubscriptionFactory(ISubscriptionKeyGenerator subscriptionKeyGenerator, IDataFabricTransport transport) : ISubscriptionFactory
{
    private readonly ISubscriptionKeyGenerator _subscriptionKeyGenerator = subscriptionKeyGenerator;
    private readonly IDataFabricTransport _transport = transport;

    public ISubscription Create(IRequestContext requestContext, string subscriptionPath) =>
        new SignalRSubscription(
            subscriptionPath,
            _subscriptionKeyGenerator.GenerateKey(requestContext),
            _transport);
}