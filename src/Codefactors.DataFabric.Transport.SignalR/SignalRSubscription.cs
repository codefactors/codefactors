// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.DataFabric.Transport.SignalR;

public class SignalRSubscription(string subscriptionPath, string subscriptionKey, IDataFabricTransport transport) : ISubscription
{
    private readonly string _subscriptionPath = subscriptionPath;
    private readonly string _subscriptionKey = subscriptionKey;
    private readonly IDataFabricTransport _transport = transport;

    public async Task NotifyAsync(string subscriptionPath, object update)
    {
        await _transport.SendAsync(_subscriptionKey, _subscriptionPath, update);
    }

    bool IEquatable<ISubscription>.Equals(ISubscription? other) =>
        other is SignalRSubscription subscription &&
        subscription._subscriptionKey == _subscriptionKey &&
        subscription._subscriptionPath == _subscriptionPath;
}
