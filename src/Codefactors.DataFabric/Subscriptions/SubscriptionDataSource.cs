// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using System.Reflection;

namespace Codefactors.DataFabric.Subscriptions;

/// <summary>
/// Entity that represents a subscription data source.
/// </summary>
public class SubscriptionDataSource(object instance, string methodName) : ISubscriptionDataSource
{
    /// <summary>
    /// Gets the instance of the data source.
    /// </summary>
    public object Instance { get; } = instance;

    /// <summary>
    /// Gets the method info of the data source method.
    /// </summary>
    public MethodInfo MethodInfo { get; } = instance.GetType().GetMethod(methodName) ??
            throw new ArgumentException($"Unable to resolve method name '{methodName}' from object of type {instance.GetType().Name}", nameof(methodName));

    /// <summary>
    /// Invokes the data source to retrieve the data specified by the supplied parameters.
    /// </summary>
    /// <param name="parameters">Parameters that enable identification and/or filtering of the required data.</param>
    /// <returns>Date item(s) requested.</returns>
    public async Task<object> InvokeAsync(params object[] parameters)
    {
        var t = (Task?)MethodInfo.Invoke(Instance, parameters) ??
            throw new InvalidOperationException("Failed to make task");

        // See https://devblogs.microsoft.com/dotnet/configureawait-faq/
        await t.ConfigureAwait(false);

        // Necessary hokeyness to get from Task<T> to Task<object>
        return (object)((dynamic)t).Result;
    }
}