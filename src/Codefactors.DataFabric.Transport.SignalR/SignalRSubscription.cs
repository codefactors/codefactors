// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Codefactors.DataFabric.Subscriptions;

namespace Codefactors.DataFabric.Transport.SignalR;

/// <summary>
/// Represents a SignalR subscription.
/// </summary>
/// <param name="subscriptionPath">Subscription path.</param>
/// <param name="subscriptionKey">Key for this subscription.</param>
/// <param name="transport">Data fabric transport.</param>
public class SignalRSubscription(string subscriptionPath, string subscriptionKey, IDataFabricTransport transport) : ISubscription
{
    private readonly string _subscriptionPath = subscriptionPath;
    private readonly string _subscriptionKey = subscriptionKey;
    private readonly IDataFabricTransport _transport = transport;

    /// <summary>Gets the key for this subscription.</summary>
    public string Key => _subscriptionKey;

    /// <summary>
    /// Notifies the subscription of an update.
    /// </summary>
    /// <param name="subscriptionPath">Path for the subscription being updated.</param>
    /// <param name="update">Update content.</param>
    /// <returns><see cref="Task"/>.</returns>
    public async Task NotifyAsync(string subscriptionPath, object update)
    {
        await _transport.SendAsync(_subscriptionKey, _subscriptionPath, update);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">Other object.</param>
    /// <returns>True if the objects are equivalent; false otherwise.</returns>
    bool IEquatable<ISubscription>.Equals(ISubscription? other) =>
        other is SignalRSubscription subscription &&
        subscription._subscriptionKey == _subscriptionKey &&
        subscription._subscriptionPath == _subscriptionPath;

    /// <summary>
    /// Returns the subscription key.
    /// </summary>
    /// <returns>Subscription key.</returns>
    public override string ToString() => _subscriptionKey;
}
