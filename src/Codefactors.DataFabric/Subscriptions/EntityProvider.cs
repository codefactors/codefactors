// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using System.Reflection;

namespace Codefactors.DataFabric.Subscriptions;

public class EntityProvider(object instance, string methodName) : IEntityProvider
{
    public object Instance { get; } = instance;

    public MethodInfo MethodInfo { get; } = instance.GetType().GetMethod(methodName) ??
            throw new ArgumentException($"Unable to resolve method name '{methodName}' from object of type {instance.GetType().Name}", nameof(methodName));

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