// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

namespace Codefactors.DataFabric.Diagnostics;

public class SubscriptionException : Exception
{
    public SubscriptionException(string message)
        : base(message)
    {
    }

    public SubscriptionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
