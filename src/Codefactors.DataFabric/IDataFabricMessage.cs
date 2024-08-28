// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.DataFabric;

/// <summary>
/// interface for data fabric messages.
/// </summary>
public interface IDataFabricMessage
{
    /// <summary>
    /// Gets the subscription path associated with this message.
    /// </summary>
    string SubscriptionPath { get; }

    /// <summary>
    /// Gets the message payload.
    /// </summary>
    object Payload { get; }
}
