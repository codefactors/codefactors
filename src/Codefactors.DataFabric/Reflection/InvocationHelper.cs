// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using Codefactors.Common.Model;
using Codefactors.DataFabric.Subscriptions;
using Microsoft.Extensions.Primitives;
using System.Reflection;

namespace Codefactors.DataFabric.Reflection;

/// <summary>
/// Helper class for invoking a method on a subscription data source.
/// </summary>
/// <param name="subscriptionDataSource">Subscription data source.</param>
/// <param name="parameters">Parameters to pass to invocation. These are extracted using the placeholders in
/// the subscription path.</param>
public class InvocationHelper(ISubscriptionDataSource subscriptionDataSource, IDictionary<string, string> parameters)
{
    /// <summary>
    /// Gets the subscription data source.
    /// </summary>
    public ISubscriptionDataSource SubscriptionDataSource { get; } = subscriptionDataSource;

    /// <summary>
    /// Gets the parameters to pass to the invocation, as a dictionary keyed on paramter name.
    /// </summary>
    public IDictionary<string, string> Parameters { get; } = parameters;

    /// <summary>
    /// Invokes the request method on the subscription data source.
    /// </summary>
    /// <param name="requestContext">Request context.</param>
    /// <param name="queryParameters">Array containing query parameters (as distinct from parameters extracted
    /// from the subscription path) as key/value pairs. May be empty.</param>
    /// <returns>Data returned by data source.</returns>
    /// <exception cref="InvalidOperationException">Thrown if any empty parameter name is supplied.</exception>
    /// <exception cref="ArgumentException">Thrown if a parameter is missing, or if an invalid value is passed
    /// for one of the parameters.</exception>
    public async Task<object> InvokeAsync(IRequestContext requestContext, IEnumerable<KeyValuePair<string, StringValues>> queryParameters)
    {
        AppendQueryParameters(queryParameters);

        var methodParameters = SubscriptionDataSource.MethodInfo.GetParameters();

        var invocationValues = new object[methodParameters.Length];

        for (int i = 0; i < methodParameters.Length; i++)
        {
            if (methodParameters[i].ParameterType == typeof(IRequestContext))
            {
                invocationValues[i] = requestContext;
            }
            else
            {
                var parameterName = methodParameters[i].Name ??
                    throw new InvalidOperationException($"Unable to invoke method; parameter #{i + 1} of method {SubscriptionDataSource.MethodInfo.Name} (instance of type {SubscriptionDataSource.Instance.GetType().Name}) returned null");

                if (Parameters.TryGetValue(parameterName, out var paramValue))
                {
                    try
                    {
                        invocationValues[i] = CoerceParameter(paramValue, methodParameters[i]);
                    }
                    catch (FormatException ex)
                    {
                        throw new ArgumentException($"Invalid value '{paramValue}' for parameter '{parameterName}'", ex);
                    }
                }
                else
                {
                    if (!methodParameters[i].HasDefaultValue)
                        throw new ArgumentException($"Missing parameter '{parameterName}'");
                }
            }
        }

        return await SubscriptionDataSource.InvokeAsync(invocationValues);
    }

    /// <summary>
    /// Gets the string representation of the invocation helper, for debug purposes.
    /// </summary>
    /// <returns>String representation of this helper.</returns>
    public override string ToString()
    {
        return $"{SubscriptionDataSource.MethodInfo.Name}({string.Join(", ", Parameters.Select(p => $"{p.Key}={p.Value}"))})";
    }

    private void AppendQueryParameters(IEnumerable<KeyValuePair<string, StringValues>> queryParameters)
    {
        foreach (var parameter in queryParameters)
            Parameters.TryAdd(parameter.Key, parameter.Value.ToString());
    }

    private object CoerceParameter(string value, ParameterInfo parameterInfo)
    {
        var type = parameterInfo.ParameterType;

        return true switch
        {
            true when type == typeof(Guid) => Guid.TryParse(value, out var guid) ? guid : throw new FormatException($"Parameter cannot be coerced to GUID, invalid format '{value}'"),
            true when type == typeof(int) => int.TryParse(value, out var intValue) ? intValue : throw new FormatException($"Parameter cannot be coerced to int, invalid format '{value}'"),
            true when type == typeof(DateTime) => DateTime.TryParse(value, out var date) ? DateTime.SpecifyKind(date, DateTimeKind.Utc) : throw new FormatException($"Parameter cannot be coerced to DateTime, invalid format '{value}'"),
            true when type == typeof(DateTime?) => DateTime.TryParse(value, out var date) ? DateTime.SpecifyKind(date, DateTimeKind.Utc) : throw new FormatException($"Parameter cannot be coerced to DateTime, invalid format '{value}'"),
            _ => value,
        };
    }
}
