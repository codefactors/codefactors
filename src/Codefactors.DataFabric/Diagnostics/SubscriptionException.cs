// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.DataFabric.Diagnostics;

/// <summary>
/// Exception thrown when an error occurs during subscription processing.
/// </summary>
public class SubscriptionException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionException"/> class.
    /// </summary>
    /// <param name="message">Exception message.</param>
    public SubscriptionException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionException"/> class.
    /// </summary>
    /// <param name="message">Exception message.</param>
    /// <param name="innerException">Inner exception.</param>
    public SubscriptionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
