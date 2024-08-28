// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.DataFabric;

/// <summary>
/// Represents a data fabric message as sent to the remote data fabric client.
/// </summary>
public class DataFabricMessage : IDataFabricMessage
{
    /// <summary>
    /// Gets the subscription path associated with this message.
    /// </summary>
    public string SubscriptionPath { get; }

    /// <summary>
    /// Gets the message payload.
    /// </summary>
    public object Payload { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataFabricMessage"/> class.
    /// </summary>
    /// <param name="subscriptionPath">Subscription path.</param>
    /// <param name="payload">Message payload.</param>
    public DataFabricMessage(string subscriptionPath, object payload)
    {
        SubscriptionPath = subscriptionPath;
        Payload = payload;
    }
}