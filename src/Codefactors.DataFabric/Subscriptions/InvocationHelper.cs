// Copyright (c) 2024, Codefactors Ltd.
//
// Codefactors Ltd licenses this file to you under the following license(s):
//
//   * The MIT License, see https://opensource.org/license/mit/

using System.Reflection;

namespace Codefactors.DataFabric.Subscriptions;

public class InvocationHelper(IEntityProvider entityProvider, IDictionary<string, string> parameters)
{
    public IEntityProvider EntityProvider { get; } = entityProvider;

    public IDictionary<string, string> Parameters { get; } = parameters;

    public async Task<object> InvokeAsync(IRequestContext requestContext)
    {
        var methodParameters = EntityProvider.MethodInfo.GetParameters();

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
                    throw new InvalidOperationException($"Unable to invoke method; parameter #{i + 1} of method {EntityProvider.MethodInfo.Name} (instance of type {EntityProvider.Instance.GetType().Name}) returned null");

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
                    throw new ArgumentException($"Missing parameter '{parameterName}'");
                }
            }
        }

        return await EntityProvider.InvokeAsync(invocationValues);
    }

    public override string ToString()
    {
        return $"{EntityProvider.MethodInfo.Name}({string.Join(", ", Parameters.Select(p => $"{p.Key}={p.Value}"))})";
    }

    private object CoerceParameter(string value, ParameterInfo parameterInfo)
    {
        var type = parameterInfo.ParameterType;

        return true switch
        {
            true when type == typeof(Guid) => Guid.TryParse(value, out var guid) ? guid : throw new FormatException($"Parameter cannot be coerced to GUID, invalid format '{value}'"),
            _ => value,
        };
    }
}
