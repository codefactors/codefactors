// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.DataFabric.Diagnostics;

/// <summary>
/// Enumeration of possible subscription error types.
/// </summary>
public enum SubscriptionErrorType
{
    /// <summary>
    /// The subscription path specified was not found.
    /// </summary>
    PathNotFound = 1,

    /// <summary>
    /// Invalid argument(s) to subsription request.
    /// </summary>
    InvalidArgument = 2,

    /// <summary>
    /// Fatal error.
    /// </summary>
    Fatal = 9
}

/// <summary>
/// Exception thrown when an error occurs during subscription processing.
/// </summary>
public class SubscriptionException : Exception
{
    /// <summary>
    /// Gets the specific subscription error type.
    /// </summary>
    public SubscriptionErrorType SubscriptionErrorType { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionException"/> class.
    /// </summary>
    /// <param name="errorType">Type of subscription error.</param>
    /// <param name="message">Exception message.</param>
    public SubscriptionException(SubscriptionErrorType errorType, string message)
        : base(message)
    {
        SubscriptionErrorType = errorType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionException"/> class.
    /// </summary>
    /// <param name="errorType">Type of subscription error.</param>
    /// <param name="message">Exception message.</param>
    /// <param name="innerException">Inner exception.</param>
    public SubscriptionException(SubscriptionErrorType errorType, string message, Exception innerException)
        : base(message, innerException)
    {
        SubscriptionErrorType = errorType;
    }
}
