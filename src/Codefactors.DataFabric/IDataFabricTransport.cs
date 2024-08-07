// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.DataFabric.Transport;

/// <summary>
/// Interface that defines the contract for a data fabric transport.
/// </summary>
public interface IDataFabricTransport
{
    /// <summary>
    /// Sends an update to a subscription.
    /// </summary>
    /// <param name="subscriptionKey">Subscription key.</param>
    /// <param name="subscriptionPath">Path of subscription.</param>
    /// <param name="update">Update content.</param>
    /// <returns><see cref="Task"/>.</returns>
    Task SendAsync(string subscriptionKey, string subscriptionPath, object update);
}