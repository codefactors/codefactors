// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using System.Reflection;

namespace Codefactors.DataFabric.Subscriptions;

/// <summary>
/// Interface that represents a subscription data source.
/// </summary>
public interface ISubscriptionDataSource
{
    /// <summary>
    /// Gets the instance of the data source.
    /// </summary>
    object Instance { get; }

    /// <summary>
    /// Gets the method info of the data source method.
    /// </summary>
    MethodInfo MethodInfo { get; }

    /// <summary>
    /// Invokes the data source to retrieve the data specified by the supplied parameters.
    /// </summary>
    /// <param name="parameters">Parameters that enable identification and/or filtering of the required data.</param>
    /// <returns>Date item(s) requested.</returns>
    Task<object> InvokeAsync(params object[] parameters);
}
